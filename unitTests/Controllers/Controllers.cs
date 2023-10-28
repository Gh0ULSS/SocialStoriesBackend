using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;
using NUnit.Framework;
using SocialStoriesBackend.Attributes;
using SocialStoriesBackend.Mappings;
using FromBodyAttribute = Microsoft.AspNetCore.Mvc.FromBodyAttribute;

namespace unitTests.Controllers;

[TestFixture]
public class Controllers
{
    private List<MethodInfo> _routeMethods = new();
    private List<Type> _routeClasses = new();
    
    private readonly List<Type> _requiredMethodAttributes = new() {
        typeof(ValidateModelAttribute),
        typeof(ConsumesAttribute),
        typeof(ProducesAttribute),
        typeof(ProducesResponseTypeAttribute)
    };
    
    private readonly List<Type> _requiredParameterAttributes = new() {
        typeof(FromBodyAttribute)
    };
    
    private readonly List<ProducesResponseTypeAttribute> _requiredMethodAuthorizedAttributes = new() {
        new ProducesResponseTypeAttribute(typeof(ErrorDto), StatusCodes.Status401Unauthorized),
        new ProducesResponseTypeAttribute(typeof(ErrorDto), StatusCodes.Status403Forbidden)
    };
    
    [OneTimeSetUp]
    public void SetUp()
    {
        
#pragma warning disable CS4014
        SocialStoriesBackend.Program.Main(new []{"SocialStoriesBackend"});
#pragma warning restore CS4014
        
        // Find classes which inherit from ControllerBase in our code
        _routeClasses = AppDomain.CurrentDomain.GetAssemblies().SelectMany(assembly => assembly.GetTypes())
            .Where(type => type.IsClass && type != typeof(ControllerBase) && typeof(ControllerBase).IsAssignableFrom(type)).ToList();
        
        // Find HTTP route methods
        _routeMethods = _routeClasses.SelectMany(type => type.GetMethods()
                .Where(method => method.GetCustomAttributes().Any(attribute => attribute is HttpMethodAttribute)))
            .ToList();
    }
    
    [Test]
    public void ControllerClasses_RouteMethods_CorrectAttributes()
    {
        // Must contain above required attributes
        // TODO: Check that routes/controllers have correct authorization policies set
        
        foreach (var method in _routeMethods) {
            var httpAttribute = method.GetCustomAttributes().FirstOrDefault(attribute => attribute is HttpMethodAttribute) as HttpMethodAttribute;
            Assert.NotNull(httpAttribute);
            
            var methodAttributes = method.GetCustomAttributes().Select(attribute => attribute).ToList();
            var methodAttributesTypes = methodAttributes.Select(attribute => attribute.GetType()).ToList();
            
            Assert.IsNotEmpty(methodAttributes);
            Assert.IsNotEmpty(methodAttributesTypes);
            
            foreach (var requiredAttributeType in _requiredMethodAttributes) {
                Assert.Contains(requiredAttributeType, methodAttributesTypes, $"Failed to find {requiredAttributeType.Name} attribute on {httpAttribute?.Template} {method.Name} method");
            }
            
            var declaringTypeAttributes = method.DeclaringType?.GetCustomAttributes().Select(attribute => attribute.GetType()).ToList();
            if ((declaringTypeAttributes is not null &&
                 declaringTypeAttributes.Contains(typeof(Microsoft.AspNetCore.Authorization.AuthorizeAttribute))) ||
                 methodAttributesTypes.Contains(typeof(Microsoft.AspNetCore.Authorization.AuthorizeAttribute))) {
                foreach (var requiredResponseAttribute in _requiredMethodAuthorizedAttributes) {
                    Assert.Contains(requiredResponseAttribute, methodAttributes, $"Failed to find \"{requiredResponseAttribute.GetType().Name}\" attribute with \"{requiredResponseAttribute.Type.Name}\" type and {requiredResponseAttribute.StatusCode} status code on \"{httpAttribute?.Template} {method.Name}\" method");
                }
            }
        }
    }
    
    [Test]
    public void ControllerClasses_RouteMethods_CorrectParameterAttributes()
    {
        // Must contain above required attributes
        foreach (var method in _routeMethods)
        {
            var parameters = method.GetParameters();
            if (parameters.Length <= 0)
                continue;
            
            Assert.AreEqual(1, parameters.Length);
            
            var httpAttribute = method.GetCustomAttributes().FirstOrDefault(attribute => attribute is HttpMethodAttribute) as HttpMethodAttribute;
            Assert.NotNull(httpAttribute);
            
            var parameterAttributes = parameters.SelectMany(parameter => parameter.CustomAttributes).Select(customAttribute => customAttribute.AttributeType).ToList();
            Assert.IsNotEmpty(parameterAttributes);
            
            foreach (var requiredAttributeType in _requiredParameterAttributes) {
                Assert.Contains(requiredAttributeType, parameterAttributes, $"Failed to find {requiredAttributeType.Name} parameter attribute on {httpAttribute?.Template} {method.Name} method");
            }
        }
    }
}