# Security Policy

## Supported Versions

We release patches for security vulnerabilities in the following versions:

| Version | Supported          |
| ------- | ------------------ |
| 1.x.x   | :white_check_mark: |

## Reporting a Vulnerability

Please report (suspected) security vulnerabilities to **security@panoramicdata.com**. You will receive a response from us within 48 hours. If the issue is confirmed, we will release a patch as soon as possible depending on complexity.

Please do not report security vulnerabilities through public GitHub issues.

## Security Considerations

When using this library:

1. **API Tokens**: Never commit API tokens to source control
2. **User Secrets**: Use User Secrets or environment variables for sensitive configuration
3. **HTTPS**: Always use HTTPS endpoints (default behavior)
4. **Token Storage**: Store tokens securely using appropriate secret management solutions

## Best Practices

- Rotate API tokens regularly
- Use read-only tokens when write access is not needed
- Implement proper error handling to avoid leaking sensitive information
- Keep the library updated to the latest version
