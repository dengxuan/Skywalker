<h1>Welcome, {{ model.user_name | html.escape }}!</h1>

<p>Thank you for joining {{ model.app_name | html.escape }}. We're excited to have you on board!</p>

{{ if model.activation_link }}
<p>Please click the button below to activate your account:</p>

<p style="text-align: center;">
    <a href="{{ model.activation_link | html.escape }}" class="button">Activate Account</a>
</p>

<p>Or copy and paste this link into your browser:</p>
<p style="word-break: break-all;">{{ model.activation_link | html.escape }}</p>

<p><small>This link will expire in {{ model.activation_link_expiration_hours }} hours.</small></p>
{{ end }}

<p>If you have any questions, feel free to contact our support team.</p>

<p>Best regards,<br>The {{ model.app_name | html.escape }} Team</p>

