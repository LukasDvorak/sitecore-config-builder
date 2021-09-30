# Sitecore Config Builder

This small console application allows you to build Sitecore configuration as you can see it on /sitecore/admin/showconfig.aspx.

## How to use it

You need to download / collect all configuration files within App_Config folder and web.config file, as they are stored in the application.

```
/App_Config
web.config
```

Compile the code and run SitecoreConfigBuilder with parameter "-path" and its value must point to the web.config file.

The output file showconfig.aspx.xml will be saved next to web.config

```
/App_Config
showconfig.aspx.xml
web.config
```

Web.config file is necessary for collecting appSettings values which are used for [rule-based configuration](https://doc.sitecore.com/en/developers/92/platform-administration-and-architecture/rule-based-configuration.html).
