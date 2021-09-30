# Sitecore Config Builder

This small console application allows you to build Sitecore configuration as you can see it on /sitecore/admin/showconfig.aspx.

## How to use it

You need to download / collect configuration files as they are in the application.

```
/App_Config
web.config
```

Compile the code and run SitecoreConfigBuilder with parameter "-path" and his value must point on web.config file.

The output file showconfig.aspx.xml will be saved next to web.config

```
/App_Config
showconfig.aspx.xml
web.config
```
