using Microsoft.AspNetCore.Mvc.RazorPages;
using System;

namespace GraphWebProject.Pages
{
    public class IndexModel : PageModel
    {
        public string Message { get; private set; } = "PageModel in C#";

        public void OnGet()
        {
            Message += $" Server time is { DateTime.Now }";
        }
    }
}