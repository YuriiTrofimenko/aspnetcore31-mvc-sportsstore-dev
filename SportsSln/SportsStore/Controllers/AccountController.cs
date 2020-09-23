using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SportsStore.Models.ViewModels;

namespace SportsStore.Controllers
{
  public class AccountController : Controller
  {
    private UserManager<IdentityUser> userManager;
    private SignInManager<IdentityUser> signInManager;
    public AccountController(UserManager<IdentityUser> userMgr,
    SignInManager<IdentityUser> signInMgr)
    {
      userManager = userMgr;
      signInManager = signInMgr;
    }
    // возвращает представление с формой входа
    // в ответ на гет-запрос
    public ViewResult Login(string returnUrl)
    {
      return View(new LoginModel
      {
        ReturnUrl = returnUrl
      });
    }
    // принимает данные формы из тела пост-запроса,
    // когда пользователь пытается войти в учетную запись
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginModel loginModel)
    {
      // выполнены ли условия валидации,
      // заданные в классе модели?
      if (ModelState.IsValid)
      {
        IdentityUser user =
          await userManager.FindByNameAsync(loginModel.Name);
        if (user != null)
        {
          await signInManager.SignOutAsync();
          if ((await signInManager.PasswordSignInAsync(user,
          loginModel.Password, false, false)).Succeeded)
          {
            return Redirect(loginModel?.ReturnUrl ?? "/Admin");
          }
        }
      }
      ModelState.AddModelError("", "Invalid name or password");
      return View(loginModel);
    }
    [Authorize]
    public async Task<RedirectResult> Logout(string returnUrl = "/")
    {
      await signInManager.SignOutAsync();
      return Redirect(returnUrl);
    }
  }
}