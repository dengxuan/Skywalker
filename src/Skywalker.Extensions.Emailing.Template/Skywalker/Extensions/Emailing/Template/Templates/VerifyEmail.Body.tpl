<h1>Verify Your Email Address</h1>

<p>Hi {{ model.user_name | html.escape }},</p>

<p>Please verify your email address for your {{ model.app_name | html.escape }} account.</p>

{{ if model.verification_link }}
<p>Click the button below to verify your email:</p>

<p style="text-align: center;">
    <a href="{{ model.verification_link | html.escape }}" class="button">Verify Email</a>
</p>

<p>Or copy and paste this link into your browser:</p>
<p style="word-break: break-all;">{{ model.verification_link | html.escape }}</p>
{{ end }}

{{ if model.verification_code }}
<p>Or enter this verification code:</p>
<p style="text-align: center; font-size: 24px; font-weight: bold; letter-spacing: 4px;">{{ model.verification_code | html.escape }}</p>
{{ end }}

<p><small>This verification will expire in {{ model.expiration_hours }} hours.</small></p>

<p>If you didn't create an account with us, you can safely ignore this email.</p>

<p>Best regards,<br>The {{ model.app_name | html.escape }} Team</p>

