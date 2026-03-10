<h1>{{ model.title | html.escape }}</h1>

<p>Hi {{ model.user_name | html.escape }},</p>

<p>{{ model.message | html.escape }}</p>

{{ if model.action_url }}
<p style="text-align: center;">
    <a href="{{ model.action_url | html.escape }}" class="button">{{ (model.action_text ?? "View Details") | html.escape }}</a>
</p>
{{ end }}

{{ if model.timestamp }}
<p><small>This notification was sent on {{ model.timestamp | date.to_string "%Y-%m-%d %H:%M" }}.</small></p>
{{ end }}

<p>Best regards,<br>The {{ model.app_name | html.escape }} Team</p>

