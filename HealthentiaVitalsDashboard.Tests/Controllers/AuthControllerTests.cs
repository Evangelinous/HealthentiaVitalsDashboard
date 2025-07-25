using Moq;
using Microsoft.AspNetCore.Identity;
using HealthentiaVitalsDashboard.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

public class AuthControllerTests
{
    private readonly Mock<UserManager<IdentityUser>> _userManagerMock;
    private readonly Mock<SignInManager<IdentityUser>> _signInManagerMock;
    private readonly Mock<IConfiguration> _configMock;
    private readonly AuthController _controller;

    public AuthControllerTests()
    {
        var userStoreMock = new Mock<IUserStore<IdentityUser>>();
        _userManagerMock = new Mock<UserManager<IdentityUser>>(userStoreMock.Object, null, null, null, null, null, null, null, null);
        
        var contextAccessor = new Mock<Microsoft.AspNetCore.Http.IHttpContextAccessor>();
        var userClaimsPrincipalFactory = new Mock<IUserClaimsPrincipalFactory<IdentityUser>>();
        _signInManagerMock = new Mock<SignInManager<IdentityUser>>(
            _userManagerMock.Object, contextAccessor.Object, userClaimsPrincipalFactory.Object, null, null, null, null
        );

        var mockConfig = new Mock<IConfiguration>(MockBehavior.Strict);

        _configMock = new Mock<IConfiguration>();
        _configMock.Setup(x => x["Jwt:Key"]).Returns("ThisIsASuperSecretKeyWith32CharsAndItIsUsedForJWT");
        _configMock.Setup(x => x["Jwt:Issuer"]).Returns("TestIssuer");
        _configMock.Setup(x => x["Jwt:Audience"]).Returns("TestAudience");

        _controller = new AuthController(_userManagerMock.Object, _signInManagerMock.Object, _configMock.Object);
    }

    [Fact]
    public async Task Login_ReturnsBadRequest_IfRequestIsNull()
    {
        // Act
        var result = await _controller.Login(null);

        // Assert
        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public async Task Login_ReturnsUnauthorized_IfUserNotFound()
    {
        _userManagerMock.Setup(um => um.FindByEmailAsync(It.IsAny<string>())).ReturnsAsync((IdentityUser?)null);

        var loginRequest = new LoginRequest { Email = "test@example.com", Password = "123456" };
        var result = await _controller.Login(loginRequest);

        Assert.IsType<UnauthorizedObjectResult>(result);
    }

    [Fact]
    public async Task Login_ReturnsUnauthorized_IfInvalidPassword()
    {
        var user = new IdentityUser { Email = "test@example.com", UserName = "test" };

        _userManagerMock.Setup(um => um.FindByEmailAsync(user.Email)).ReturnsAsync(user);
        _signInManagerMock.Setup(sm => sm.CheckPasswordSignInAsync(user, "wrongpass", false))
                        .ReturnsAsync(Microsoft.AspNetCore.Identity.SignInResult.Failed);

        var loginRequest = new LoginRequest { Email = user.Email, Password = "wrongpass" };
        var result = await _controller.Login(loginRequest);

        Assert.IsType<UnauthorizedObjectResult>(result);
    }

    [Fact]
    public async Task Login_ReturnsOk_WithToken_IfSuccessful()
    {
        var user = new IdentityUser { Email = "test@example.com", UserName = "test" };

        _userManagerMock.Setup(um => um.FindByEmailAsync(user.Email)).ReturnsAsync(user);
        _signInManagerMock.Setup(sm => sm.CheckPasswordSignInAsync(user, "correctpass", false))
                          .ReturnsAsync(Microsoft.AspNetCore.Identity.SignInResult.Success);

        var loginRequest = new LoginRequest { Email = user.Email, Password = "correctpass" };
        var result = await _controller.Login(loginRequest);

        var okResult = Assert.IsType<OkObjectResult>(result);
        var tokenObj = okResult.Value?.GetType().GetProperty("token")?.GetValue(okResult.Value);
        Assert.NotNull(tokenObj);
        Assert.IsType<string>(tokenObj);
    }
}