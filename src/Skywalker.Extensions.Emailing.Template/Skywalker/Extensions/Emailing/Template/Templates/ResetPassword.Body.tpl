<h1>Password Reset Request</h1>

<p>Hi {{ model.user_name | html.escape }},</p>

<p>We received a request to reset your password for your {{ model.app_name | html.escape }} account.</p>

<p>Click the button below to reset your password:</p>

<p style="text-align: center;">
    <a href="{{ model.reset_link | html.escape }}" class="button">Reset Password</a>
</p>

<p>Or copy and paste this link into your browser:</p>
<p style="word-break: break-all;">{{ model.reset_link | html.escape }}</p>

<p><small>This link will expire in {{ model.reset_link_expiration_hours }} hours.</small></p>

<p>If you didn't request a password reset, you can safely ignore this email. Your password will remain unchanged.</p>

<p>Best regards,<br>The {{ model.app_name | html.escape }} Team</p>

