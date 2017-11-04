# LinkedSharp
Linkedin C# API SDK

This is an open source 3rd party C# SDK to connect with <a href="https://developer.linkedin.com/docs"> Linkedin API</a> for get full profile (or company pages) info by <a href="https://developer.linkedin.com/docs/signin-with-linkedin">Sign In with LinkedIn</a> with OAuth2 2017 and <a href="https://developer.linkedin.com/docs/share-on-linkedin"> share post </a> or <a href="https://developer.linkedin.com/docs/company-pages">Manage company pages</a> easily.

<hr>
<h2>Example</h2>

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





