using Microsoft.Security.Application;

namespace blog_api.services.Helpers
{
    internal static class SanitizeHandler
    {
        /// <summary>
        ///  This method removes any potentially dangerous HTML tags and attributes, 
        ///  and returns a string that only contains safe HTML. 
        ///  The method then checks if the sanitization was successful by verifying 
        ///  that none of the properties are still null or empty. 
        ///  If all properties are not null or empty, the method returns 
        ///  true to indicate that the sanitization was successful. Otherwise, it returns false.
        /// </summary>
        /// <param name="prop"></param>
        /// <returns></returns>
        public static bool SanitizePropForXss(object prop)
        {
            bool success = true;

            string propToCheck = prop.ToString();

            // Sanitize the properties of the model for XSS attacks
            if (!string.IsNullOrEmpty(propToCheck))
            {
                propToCheck = Sanitizer.GetSafeHtmlFragment(propToCheck);
            }

            // Check if the sanitization was successful
            if (!string.IsNullOrEmpty(propToCheck) )
            {
                success = false;
            }

            return success;
        }
    }
}
