using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CBTDWeb.Pages.Roles
{
    [Authorize(Roles = "Instructor")]
    public class UpsertModel : PageModel
    {
        private readonly RoleManager<IdentityRole> _roleManager;

        public UpsertModel(RoleManager<IdentityRole> roleManager)
        {
            _roleManager = roleManager;
        }

        [BindProperty]
        public IdentityRole CurrentRole { get; set; }
        [BindProperty]
        public bool IsUpdate { get; set; }

        public async Task OnGetAsync(string? id)
        {
            if (id != null)
            {
                CurrentRole = await _roleManager.FindByIdAsync(id);
                IsUpdate = true;
            }
            else
            {
                CurrentRole = new IdentityRole();
                IsUpdate = false;
            }
        }

        public async Task<IActionResult> OnPostAsync()
        {

            if (!IsUpdate)
            {
                CurrentRole.NormalizedName = CurrentRole.Name.ToUpper();
                TempData["success"] = "Successfully Added";
                await _roleManager.CreateAsync(CurrentRole);
                return RedirectToPage("./Index");
            }
            else
            {
                CurrentRole.NormalizedName = CurrentRole.Name.ToUpper();
                TempData["success"] = "Update Successful";
                await _roleManager.UpdateAsync(CurrentRole);
                return RedirectToPage("./Index");
            }

        }
    }
}