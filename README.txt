
- Login with this site issues two cookies
     - auth cookie for the site itself (aspnet identity cookie)
     - auth cookie for mod_auth_pubtkt (or similiar)
- Needs to support
    DONE - Login with aspnet identity
    DONE - Issue mod_auth_pubtkt
    TODO - Issue something for nginx?
    DONE - Validate mod_auth_pubtkt tickets
    TODO - Configure loginUrl for middleware
    TODO - Method to renew tickets without redirect back to login page (for reverse proxy scenario where mod_auth_pubtkt is on proxy)
                - Maybe a refresh callback that the proxy UI can call whenever there is activity or some crap?
                - Maybe a mod_perl module that can re-issue the ticket with a new validuntil?
    TODO - Middleware for mod_auth_pubtkt
    TODO - Configuration for pubtkt private key
    TODO - MySQL Support
    TODO - Multi-Factor auth via TOTP/HOTP
    TODO - Multi-Factor auth via text
    TODO - Custom IUserStore / IRoleStore support (for existing Mirkweb db)
    TODO - Self host a proxy using reverse middleware instead of using Apache reverse proxy using github/aspnet/proxy