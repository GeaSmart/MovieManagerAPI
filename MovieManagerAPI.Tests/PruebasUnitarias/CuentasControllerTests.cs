using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MovieManagerAPI.Controllers;
using MovieManagerAPI.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MovieManagerAPI.Tests.PruebasUnitarias
{
    [TestClass]
    public class CuentasControllerTests : BasePruebas
    {
        [TestMethod]
        public async Task CrearUsuario()
        {
            var nombreBD = Guid.NewGuid().ToString();
            await CrearUsuarioHelper(nombreBD);

            var contexto2 = ConstruirContext(nombreBD);
            var conteo = await contexto2.Users.CountAsync();
            Assert.AreEqual(1, conteo);
        }

        [TestMethod]
        public async Task UsuarioNoPuedeLoguearse()
        {
            var nombreBD = Guid.NewGuid().ToString();
            await CrearUsuarioHelper(nombreBD);

            var controller = ConstruirCuentasController(nombreBD);
            var userInfo = new UserInfo() { Email = "ejemplo@ejemplo.com", Password = "password-equivocado" };

            var respuesta = await controller.Login(userInfo);

            Assert.IsNull(respuesta.Value);
            var resultado = respuesta.Result as BadRequestObjectResult;

            Assert.IsNotNull(resultado);
        }

        [TestMethod]
        public async Task UsuarioPuedeLoguearse()
        {
            var nombreBD = Guid.NewGuid().ToString();
            await CrearUsuarioHelper(nombreBD);

            var controller = ConstruirCuentasController(nombreBD);
            var userInfo = new UserInfo() { Email = "ejemplo@ejemplo.com", Password = "Aa123456!" };

            var respuesta = await controller.Login(userInfo);

            Assert.IsNotNull(respuesta.Value);
            Assert.IsNotNull(respuesta.Value.Token);
        }

        //métodos auxiliares
        public async Task CrearUsuarioHelper(string nombreBD)
        {
            var cuentasController = ConstruirCuentasController(nombreBD);
            var userInfo = new UserInfo() { Email = "ejemplo@ejemplo.com", Password = "Aa123456!" };//la contraseña debe cumplir politicas, sino no crea el usuario
            await cuentasController.Registrar(userInfo);
        }

        private CuentasController ConstruirCuentasController(string nombreBD)
        {
            var context = ConstruirContext(nombreBD);
            var miUserStore = new UserStore<IdentityUser>(context);
            var userManager = BuildUserManager(miUserStore);
            var mapper = ConfigurarAutomapper();

            var httpContext = new DefaultHttpContext();
            MockAuth(httpContext);
            var signInManager = SetupSignInManager(userManager, httpContext);

            var miConfiguracion = new Dictionary<string, string>
            {
                {"JWT:key", "FSD1ADF6GH1FG1RT814UYTNMVBR1IOUP1K16HF1W1SFD1H6N8G6FD1J8QREHPIUG61JGD16GFDJA1FGDSDFJJK33I1JU8I1DG00" }
            };//el mismo key que está en appsettings.Development.json, aunque no es obligatorio que sea el mismo.

            var configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(miConfiguracion)
                .Build();

            return new CuentasController(userManager, signInManager, configuration, context, mapper);
        }

        private Mock<IAuthenticationService> MockAuth(HttpContext context)
        {
            var auth = new Mock<IAuthenticationService>();
            context.RequestServices = new ServiceCollection().AddSingleton(auth.Object).BuildServiceProvider();
            return auth;
        }

        /* ++++++++++ STANDARD METHODS FROM DOT NET ++++++++++*/
        // Source: https://github.com/dotnet/aspnetcore/blob/master/src/Identity/test/Shared/MockHelpers.cs
        // Source: https://github.com/dotnet/aspnetcore/blob/master/src/Identity/test/Identity.Test/SignInManagerTest.cs
        // Some code was modified to be adapted to our project.
        // He traído estos métodos del repo oficial de .net en github, usaremos estos métodos a modo de caja negra, no nos importa mucho
        // cómo estan hechos, simplemente haremos uso de ellos, y también he tenido que importar sus librerías con using en la parte superior.

        private UserManager<TUser> BuildUserManager<TUser>(IUserStore<TUser> store = null) where TUser : class
        {
            store = store ?? new Mock<IUserStore<TUser>>().Object;
            var options = new Mock<IOptions<IdentityOptions>>();
            var idOptions = new IdentityOptions();
            idOptions.Lockout.AllowedForNewUsers = false;

            options.Setup(o => o.Value).Returns(idOptions);

            var userValidators = new List<IUserValidator<TUser>>();

            var validator = new Mock<IUserValidator<TUser>>();
            userValidators.Add(validator.Object);
            var pwdValidators = new List<PasswordValidator<TUser>>();
            pwdValidators.Add(new PasswordValidator<TUser>());

            var userManager = new UserManager<TUser>(store, options.Object, new PasswordHasher<TUser>(),
                userValidators, pwdValidators, new UpperInvariantLookupNormalizer(),
                new IdentityErrorDescriber(), null,
                new Mock<ILogger<UserManager<TUser>>>().Object);

            validator.Setup(v => v.ValidateAsync(userManager, It.IsAny<TUser>()))
                .Returns(Task.FromResult(IdentityResult.Success)).Verifiable();

            return userManager;
        }

        private static SignInManager<TUser> SetupSignInManager<TUser>(UserManager<TUser> manager,
            HttpContext context, ILogger logger = null, IdentityOptions identityOptions = null,
            IAuthenticationSchemeProvider schemeProvider = null) where TUser : class
        {
            var contextAccessor = new Mock<IHttpContextAccessor>();
            contextAccessor.Setup(a => a.HttpContext).Returns(context);
            identityOptions = identityOptions ?? new IdentityOptions();
            var options = new Mock<IOptions<IdentityOptions>>();
            options.Setup(a => a.Value).Returns(identityOptions);
            var claimsFactory = new UserClaimsPrincipalFactory<TUser>(manager, options.Object);
            schemeProvider = schemeProvider ?? new Mock<IAuthenticationSchemeProvider>().Object;
            var sm = new SignInManager<TUser>(manager, contextAccessor.Object, claimsFactory, options.Object, null, schemeProvider, new DefaultUserConfirmation<TUser>());
            sm.Logger = logger ?? (new Mock<ILogger<SignInManager<TUser>>>()).Object;
            return sm;
        }

    }
}
