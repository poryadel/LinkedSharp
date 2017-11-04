# LinkedSharp
Linkedin C# API SDK

This is an open source 3rd party C# SDK to connect with <a href="https://developer.linkedin.com/docs"> Linkedin API</a> for get full profile (or company pages) info by <a href="https://developer.linkedin.com/docs/signin-with-linkedin">Sign In with LinkedIn</a> with OAuth2 2017 and <a href="https://developer.linkedin.com/docs/share-on-linkedin"> share post </a> or <a href="https://developer.linkedin.com/docs/company-pages">Manage company pages</a> easily.

<hr>
<h2>Example</h2>
<h5>Post and share some Text to your linkedin account</h5>

```
Linkedin.LinkedInTextPost newPost = new Linkedin.LinkedInTextPost()
{
    comment = "Your text post",
    visibility = new Linkedin.Visibility
    {
        code = "anyone"
    }
};
   var Update = await Linkedin.Post.UpdateStatusAsync("Token", newPost);
```
<hr>
<h5>Get Linkedin account full information</h5>

```
var fullProfileInformation = await Linkedin.Profile.GetInfoAsync("accessToken");
```
<h6>And the result:</h6>

![get data from Linkedin](https://kookkon.com/images/linkedin.png)





