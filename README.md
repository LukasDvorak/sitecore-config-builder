# Sitecore Config Builder

This small console application allows you to build Sitecore configuration as you can see it on /sitecore/admin/showconfig.aspx withour Sitecore instance. It could be handy when you cannot access showconfig.aspx page, for example on CD server.

## How to use it

You need to download / collect all configuration files within App_Config folder and web.config file, as they are stored in the application.

```
/App_Config
/web.config
```

Compile the code and run SitecoreConfigBuilder with parameter **-path** and its value must point to the web.config file.

The output file **showconfig.aspx.xml** will be saved next to web.config

```
/App_Config
/showconfig.aspx.xml
/web.config
```

Web.config file is necessary to access Sitecore configuration part

```
<sitecore configSource="App_Config\Sitecore.config"/>
```

and for collecting appSettings values

```
<appSettings>
  <add key="role:define" value="Standalone" />
  <add key="search:define" value="Solr" />
  ...
</appSettings>
```

which are used for [rule-based configuration](https://doc.sitecore.com/en/developers/92/platform-administration-and-architecture/rule-based-configuration.html).
