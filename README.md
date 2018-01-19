# Modules for Orchard CMS

##  dcp.Routing

Extends routing system.

### dcp.Routing feature
You can create a custom route in MVC style to point to existing content item. The feature extends exisitng alias system, thus a route can be linked to existing alias of content item.

For example you have Projection page that use params from query string to filter some query. Url like _/products?category=XXX_.

Then you can migrate these params to route data and create a route through the UI dashboard like:
_/{category}/products_. This route will point to your existing projection content item.

You can create routes that linked to any content types with **AutoRoute** part.

### dcp.Routing.Redirects feature
The feature allow to manage redirect rules. You can create/update/delete via UI of admin dashboard.
Additionaly you can migrate these rules IIS rewrite module section of _web.config_ (if the IIS module is enabled on your host environment).
Allows 301/302 redirects to be configured for changed URLs.

### dcp.Routing.UrlUpdating feature
The feature allow to automatically create redirect rules when diplay url of your content is changed. This is useful SEO purposes.

## dcp.Dropzone
Add dropzone file uploader functionality.

Features:

1. Add dropzone scripts and style to resources
1. Replace AgileUploaderField template with dropzone

## dcp.jQueryValidate

JavaScript resources of jQuery validation plugin.

This moduile is a fork from MidnightPixel.jQueryValidate to use in Orchard >= 1.10.1

## dcp.Meta

Upgraded Vandelay.Meta to Orchard 1.10.1

## Agile Uploader Field

Upgraded version for Orchard 1.10 and later

## SH.Robots

Upgraded SH.Robots to Orchard 1.10.1

## WebAdvanced.Sitemap

Upgraded WebAdvanced.Sitemap module to use with Orchard >= 1.10.1

## Orchard.ContentTree

Allow better manage content items using tree view.

Upgraded Orchard.ContentTree Module to use with Orchard >= 1.10.1

## SH.GoogleAnalytics

Upgraded https://gallery.orchardproject.net/Packages/Orchard.Module.SH.GoogleAnalytics to meet requirements Orchard 1.10.1

## dcp.Utility

Add many useful utils and extends some Orchard features:

* Projection: sort by querystring param.
* Database layer: 
   * base content repository 
   * data filters:
     * by email
     * by content field
     * by owner
     * by term
     * by title
   * data sorting
     * by title
* Tokens: any request param
* Generic helper extenstions
  * content GetField
  * authorize: AuthorizeAll permissions,AuthorizeAny permissions
* Security Shape provider: configurate which shapes show or hide depends on permissions
* UpdatableControllerBase: more friendly base controller that binds its input data
* ViewModelContentItemBuilder: to quick build a composite shape that contains parts/shapes from other content items (usually uses as View Model so not need create a separate class) 

## dcp.WebApiHelpPage

Add Web Api Help pages (https://docs.microsoft.com/en-us/aspnet/web-api/overview/getting-started-with-aspnet-web-api/creating-api-help-pages) for all public Api of the site.
