# Script to add XML documentation comments to controllers

$docs = @{
    "AuthController.cs" = @{
        "Login" = "/// <summary>`r`n        /// Authenticates user and returns JWT token`r`n        /// </summary>"
        "RefreshToken" = "/// <summary>`r`n        /// Refreshes expired JWT token using refresh token`r`n        /// </summary>"
        "ChangePassword" = "/// <summary>`r`n        /// Changes authenticated user's password`r`n        /// </summary>"
        "ForgotPassword" = "/// <summary>`r`n        /// Initiates password reset process via email`r`n        /// </summary>"
        "ResetPassword" = "/// <summary>`r`n        /// Resets user password using reset token`r`n        /// </summary>"
        "SendVerificationEmail" = "/// <summary>`r`n        /// Sends email verification link to user`r`n        /// </summary>"
        "ConfirmEmail" = "/// <summary>`r`n        /// Confirms user email address using verification token`r`n        /// </summary>"
        "SendPhoneVerification" = "/// <summary>`r`n        /// Sends OTP code to phone for verification`r`n        /// </summary>"
        "ConfirmPhoneVerification" = "/// <summary>`r`n        /// Verifies phone number using OTP code`r`n        /// </summary>"
        "ExternalGoogleLogin" = "/// <summary>`r`n        /// Authenticates user via Google OAuth`r`n        /// </summary>"
    }
    "AdminsController.cs" = @{
        "ListAdmins" = "/// <summary>`r`n        /// Lists all administrators with pagination`r`n        /// </summary>"
        "GetDashboard" = "/// <summary>`r`n        /// Retrieves admin dashboard with system statistics`r`n        /// </summary>"
    }
    "CartsController.cs" = @{
        "GetCart" = "/// <summary>`r`n        /// Retrieves authenticated customer's shopping cart`r`n        /// </summary>"
        "AddToCart" = "/// <summary>`r`n        /// Adds a service to customer's cart`r`n        /// </summary>"
        "RemoveFromCart" = "/// <summary>`r`n        /// Removes a service from customer's cart`r`n        /// </summary>"
        "ClearCart" = "/// <summary>`r`n        /// Clears all items from customer's cart`r`n        /// </summary>"
        "Checkout" = "/// <summary>`r`n        /// Checks out cart and creates an order`r`n        /// </summary>"
        "BuyNow" = "/// <summary>`r`n        /// Creates immediate order without cart`r`n        /// </summary>"
    }
    "CategoriesController.cs" = @{
        "ListCategories" = "/// <summary>`r`n        /// Lists all service categories with pagination`r`n        /// </summary>"
    }
    "PaymentsController.cs" = @{
        "ProcessPaymobWebhook" = "/// <summary>`r`n        /// Processes Paymob payment webhook notifications`r`n        /// </summary>"
        "Health" = "/// <summary>`r`n        /// Health check endpoint for payment service`r`n        /// </summary>"
    }
}

$controllersPath = "c:\Users\egysc\Documents\Work\GitHub\Spotless\src\Spotless.API\Controllers"

foreach ($file in $docs.Keys) {
    $filePath = Join-Path $controllersPath $file
    $content = Get-Content $filePath -Raw
    
    foreach ($method in $docs[$file].Keys) {
        $doc = $docs[$file][$method]
        # Add doc before [HttpGet], [HttpPost], etc.
        $content = $content -replace "(\r\n\s+)(\[Http)", "`$1$doc`r`n        `$2"
    }
    
    Set-Content $filePath $content -NoNewline
}

Write-Host "XML documentation added successfully!"
