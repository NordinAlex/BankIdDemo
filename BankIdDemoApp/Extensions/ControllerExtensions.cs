using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Mvc;

namespace BankIdDemoApp.Extensions
{
    public static class ControllerExtensions
    {
        public static async Task<string> RenderViewAsync(this Controller controller, string viewName, object model, bool partial = false)
        {
            controller.ViewData.Model = model;
            using (var writer = new StringWriter())
            {
                var viewResult = controller.ViewEngine().FindView(controller.ControllerContext, viewName, !partial);
                if (viewResult.View == null)
                {
                    throw new FileNotFoundException($"View '{viewName}' not found.");
                }
                var viewContext = new ViewContext(
                    controller.ControllerContext,
                    viewResult.View,
                    controller.ViewData,
                    controller.TempData,
                    writer,
                    new HtmlHelperOptions()
                );
                await viewResult.View.RenderAsync(viewContext);
                return writer.GetStringBuilder().ToString();
            }
        }

        private static ICompositeViewEngine ViewEngine(this Controller controller)
        {
            return (ICompositeViewEngine)controller.HttpContext.RequestServices.GetService(typeof(ICompositeViewEngine));
        }
    }
}
