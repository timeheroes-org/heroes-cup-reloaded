﻿using Piranha.AttributeBuilder;
using Piranha.Models;

namespace HeroesCup.Web.Models;

[PageType(Title = "Landing page")]
[ContentTypeRoute(Title = "Landing", Route = "/landing")]
public class LandingPage : Page<LandingPage>, ISocialNetworkPost
{
    public SocialNetworksMetaData SocialNetworksMetaData { get; set; }
}