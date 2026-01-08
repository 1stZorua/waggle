{{- define "fluent-bit.name" -}}
fluent-bit
{{- end -}}

{{- define "fluent-bit.fullname" -}}
{{ .Release.Name }}-fluent-bit
{{- end -}}

{{- define "fluent-bit.serviceAccountName" -}}
{{ include "fluent-bit.fullname" . }}
{{- end -}}