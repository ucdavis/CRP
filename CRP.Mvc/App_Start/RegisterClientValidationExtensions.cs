using DataAnnotationsExtensions.ClientValidation;

[assembly: WebActivator.PreApplicationStartMethod(typeof(CRP.Mvc.App_Start.RegisterClientValidationExtensions), "Start")]
 
namespace CRP.Mvc.App_Start {
    public static class RegisterClientValidationExtensions {
        public static void Start() {
            DataAnnotationsModelValidatorProviderExtensions.RegisterValidationExtensions();            
        }
    }
}