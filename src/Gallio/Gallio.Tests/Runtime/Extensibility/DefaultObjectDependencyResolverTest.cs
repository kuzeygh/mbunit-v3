﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using Gallio.Collections;
using Gallio.Runtime;
using Gallio.Runtime.Extensibility;
using MbUnit.Framework;
using Rhino.Mocks;

namespace Gallio.Tests.Runtime.Extensibility
{
    [TestsOn(typeof(DefaultObjectDependencyResolver))]
    public class DefaultObjectDependencyResolverTest
    {
        public class ArgumentValidations
        {
            [Test]
            public void Constructor_WhenServiceLocatorIsNull_Throws()
            {
                var resourceLocator = MockRepository.GenerateStub<IResourceLocator>();

                Assert.Throws<ArgumentNullException>(() => new DefaultObjectDependencyResolver(null, resourceLocator));
            }

            [Test]
            public void Constructor_WhenResourceLocatorIsNull_Throws()
            {
                var serviceLocator = MockRepository.GenerateStub<IServiceLocator>();

                Assert.Throws<ArgumentNullException>(() => new DefaultObjectDependencyResolver(serviceLocator, null));
            }

            [Test]
            public void ResolveDependency_WhenParameterNameIsNull_Throws()
            {
                var serviceLocator = MockRepository.GenerateStub<IServiceLocator>();
                var resourceLocator = MockRepository.GenerateStub<IResourceLocator>();
                var dependencyResolver = new DefaultObjectDependencyResolver(serviceLocator, resourceLocator);

                Assert.Throws<ArgumentNullException>(() => dependencyResolver.ResolveDependency(null, typeof(string), "value"));
            }

            [Test]
            public void ResolveDependency_WhenParameterTypeIsNull_Throws()
            {
                var serviceLocator = MockRepository.GenerateStub<IServiceLocator>();
                var resourceLocator = MockRepository.GenerateStub<IResourceLocator>();
                var dependencyResolver = new DefaultObjectDependencyResolver(serviceLocator, resourceLocator);

                Assert.Throws<ArgumentNullException>(() => dependencyResolver.ResolveDependency("name", null, "value"));
            }
        }

        public class ResolveByServiceLocation
        {
            [Test]
            public void ResolveDependency_WhenArgumentIsNullAndTypeIsScalar_ResolvesDependencyWithServiceLocatorByType()
            {
                var serviceLocator = MockRepository.GenerateMock<IServiceLocator>();
                var resourceLocator = MockRepository.GenerateMock<IResourceLocator>();
                var dependencyResolver = new DefaultObjectDependencyResolver(serviceLocator, resourceLocator);
                var service = MockRepository.GenerateStub<IService>();
                serviceLocator.Expect(x => x.HasService(typeof(IService))).Return(true);
                serviceLocator.Expect(x => x.Resolve(typeof(IService))).Return(service);

                var result = dependencyResolver.ResolveDependency("service", typeof(IService), null);

                Assert.Multiple(() =>
                {
                    Assert.IsTrue(result.IsSatisfied);
                    Assert.IsInstanceOfType<IService>(result.Value);
                    Assert.AreSame(service, result.Value);
                });

                serviceLocator.VerifyAllExpectations();
                resourceLocator.VerifyAllExpectations();
            }

            [Test]
            public void ResolveDependency_WhenArgumentIsNullAndTypeIsScalarOfGenericComponentHandle_ResolvesDependencyWithServiceLocatorByType()
            {
                var serviceLocator = MockRepository.GenerateMock<IServiceLocator>();
                var resourceLocator = MockRepository.GenerateMock<IResourceLocator>();
                var dependencyResolver = new DefaultObjectDependencyResolver(serviceLocator, resourceLocator);
                var componentHandle = CreateStubComponentHandle<IService, Traits>();
                serviceLocator.Expect(x => x.HasService(typeof(IService))).Return(true);
                serviceLocator.Expect(x => x.ResolveHandle(typeof(IService))).Return(componentHandle);

                var result = dependencyResolver.ResolveDependency("service", typeof(ComponentHandle<IService, Traits>), null);

                Assert.Multiple(() =>
                {
                    Assert.IsTrue(result.IsSatisfied);
                    Assert.AreSame(componentHandle, result.Value);
                });

                serviceLocator.VerifyAllExpectations();
                resourceLocator.VerifyAllExpectations();
            }

            [Test]
            public void ResolveDependency_WhenArgumentIsNullAndTypeIsScalarOfNonGenericComponentHandle_Throws()
            {
                var serviceLocator = MockRepository.GenerateMock<IServiceLocator>();
                var resourceLocator = MockRepository.GenerateMock<IResourceLocator>();
                var dependencyResolver = new DefaultObjectDependencyResolver(serviceLocator, resourceLocator);

                var ex = Assert.Throws<RuntimeException>(() => dependencyResolver.ResolveDependency("service", typeof(ComponentHandle), null));
                Assert.AreEqual("Could not detect service type from non-generic component handle.", ex.Message);
                serviceLocator.VerifyAllExpectations();
                resourceLocator.VerifyAllExpectations();
            }

            [Test]
            public void ResolveDependency_WhenArgumentIsNullAndTypeIsArray_ResolvesDependencyWithServiceLocatorByTypeAndReturnsAllMatches()
            {
                var serviceLocator = MockRepository.GenerateMock<IServiceLocator>();
                var resourceLocator = MockRepository.GenerateMock<IResourceLocator>();
                var dependencyResolver = new DefaultObjectDependencyResolver(serviceLocator, resourceLocator);
                var service1 = MockRepository.GenerateStub<IService>();
                var service2 = MockRepository.GenerateStub<IService>();
                serviceLocator.Expect(x => x.HasService(typeof(IService))).Return(true);
                serviceLocator.Expect(x => x.ResolveAll(typeof(IService))).Return(new object[] { service1, service2 });

                var result = dependencyResolver.ResolveDependency("service", typeof(IService[]), null);

                Assert.Multiple(() =>
                {
                    Assert.IsTrue(result.IsSatisfied);
                    Assert.IsInstanceOfType<IService[]>(result.Value);

                    IService[] resultValue = (IService[])result.Value;
                    Assert.AreEqual(2, resultValue.Length);
                    Assert.AreSame(service1, resultValue[0]);
                    Assert.AreSame(service2, resultValue[1]);
                });

                serviceLocator.VerifyAllExpectations();
                resourceLocator.VerifyAllExpectations();
            }

            [Test]
            public void ResolveDependency_WhenArgumentIsNullAndTypeIsArrayOfGenericComponentHandle_ResolvesDependencyWithServiceLocatorByTypeAndReturnsAllMatches()
            {
                var serviceLocator = MockRepository.GenerateMock<IServiceLocator>();
                var resourceLocator = MockRepository.GenerateMock<IResourceLocator>();
                var dependencyResolver = new DefaultObjectDependencyResolver(serviceLocator, resourceLocator);
                var componentHandle1 = CreateStubComponentHandle<IService, Traits>();
                var componentHandle2 = CreateStubComponentHandle<IService, Traits>();
                serviceLocator.Expect(x => x.HasService(typeof(IService))).Return(true);
                serviceLocator.Expect(x => x.ResolveAllHandles(typeof(IService))).Return(new ComponentHandle[] { componentHandle1, componentHandle2 });

                var result = dependencyResolver.ResolveDependency("service", typeof(ComponentHandle<IService, Traits>[]), null);

                Assert.Multiple(() =>
                {
                    Assert.IsTrue(result.IsSatisfied);

                    var resultValue = (ComponentHandle<IService, Traits>[])result.Value;
                    Assert.AreEqual(2, resultValue.Length);
                    Assert.AreSame(componentHandle1, resultValue[0]);
                    Assert.AreSame(componentHandle2, resultValue[1]);
                });

                serviceLocator.VerifyAllExpectations();
                resourceLocator.VerifyAllExpectations();
            }

            [Test]
            public void ResolveDependency_WhenArgumentIsNullAndTypeIsArrayOfNonGenericComponentHandle_Throws()
            {
                var serviceLocator = MockRepository.GenerateMock<IServiceLocator>();
                var resourceLocator = MockRepository.GenerateMock<IResourceLocator>();
                var dependencyResolver = new DefaultObjectDependencyResolver(serviceLocator, resourceLocator);

                var ex = Assert.Throws<RuntimeException>(() => dependencyResolver.ResolveDependency("service", typeof(ComponentHandle[]), null));
                Assert.AreEqual("Could not detect service type from non-generic component handle.", ex.Message);
                serviceLocator.VerifyAllExpectations();
                resourceLocator.VerifyAllExpectations();
            }

            [Test]
            public void ResolveDependency_WhenArgumentIsNullAndTypeIsScalarButServiceNotRegistered_ReturnsUnsatisfiedDependency()
            {
                var serviceLocator = MockRepository.GenerateMock<IServiceLocator>();
                var resourceLocator = MockRepository.GenerateMock<IResourceLocator>();
                var dependencyResolver = new DefaultObjectDependencyResolver(serviceLocator, resourceLocator);
                serviceLocator.Expect(x => x.HasService(typeof(IService))).Return(false);

                var result = dependencyResolver.ResolveDependency("service", typeof(IService), null);

                Assert.IsFalse(result.IsSatisfied);

                serviceLocator.VerifyAllExpectations();
                resourceLocator.VerifyAllExpectations();
            }

            [Test]
            public void ResolveDependency_WhenArgumentIsNullAndTypeIsArrayButServiceNotRegistered_ReturnsUnsatisfiedDependency()
            {
                var serviceLocator = MockRepository.GenerateMock<IServiceLocator>();
                var resourceLocator = MockRepository.GenerateMock<IResourceLocator>();
                var dependencyResolver = new DefaultObjectDependencyResolver(serviceLocator, resourceLocator);
                serviceLocator.Expect(x => x.HasService(typeof(IService))).Return(false);

                var result = dependencyResolver.ResolveDependency("service", typeof(IService[]), null);

                Assert.IsFalse(result.IsSatisfied);

                serviceLocator.VerifyAllExpectations();
                resourceLocator.VerifyAllExpectations();
            }
        }

        public class ResolveByConfiguration
        {
            [Test]
            public void ResolveDependency_WhenArgumentSpecifiesComponentId_ResolvesDependencyWithServiceLocatorUsingComponentId()
            {
                var serviceLocator = MockRepository.GenerateMock<IServiceLocator>();
                var resourceLocator = MockRepository.GenerateMock<IResourceLocator>();
                var dependencyResolver = new DefaultObjectDependencyResolver(serviceLocator, resourceLocator);
                var service = MockRepository.GenerateStub<IService>();
                serviceLocator.Expect(x => x.ResolveByComponentId("componentId")).Return(service);

                var result = dependencyResolver.ResolveDependency("service", typeof(IService), "${componentId}");

                Assert.Multiple(() =>
                {
                    Assert.IsTrue(result.IsSatisfied);
                    Assert.IsInstanceOfType<IService>(result.Value);
                    Assert.AreSame(service, result.Value);
                });

                serviceLocator.VerifyAllExpectations();
                resourceLocator.VerifyAllExpectations();
            }

            [Test]
            public void ResolveDependency_WhenArgumentSpecifiesComponentIdAndRequestedHandle_ResolvesDependencyWithServiceLocatorUsingComponentId()
            {
                var serviceLocator = MockRepository.GenerateMock<IServiceLocator>();
                var resourceLocator = MockRepository.GenerateMock<IResourceLocator>();
                var dependencyResolver = new DefaultObjectDependencyResolver(serviceLocator, resourceLocator);
                var componentHandle = CreateStubComponentHandle<IService, Traits>();
                serviceLocator.Expect(x => x.ResolveHandleByComponentId("componentId")).Return(componentHandle);

                var result = dependencyResolver.ResolveDependency("service", typeof(ComponentHandle), "${componentId}");

                Assert.Multiple(() =>
                {
                    Assert.IsTrue(result.IsSatisfied);
                    Assert.AreSame(componentHandle, result.Value);
                });

                serviceLocator.VerifyAllExpectations();
                resourceLocator.VerifyAllExpectations();
            }

            [Test]
            public void ResolveDependency_WhenArgumentSpecifiesComponentIdThatDoesNotActuallyImplementServiceType_Throws()
            {
                var serviceLocator = MockRepository.GenerateMock<IServiceLocator>();
                var resourceLocator = MockRepository.GenerateMock<IResourceLocator>();
                var dependencyResolver = new DefaultObjectDependencyResolver(serviceLocator, resourceLocator);
                var service = new object();
                serviceLocator.Expect(x => x.ResolveByComponentId("componentId")).Return(service);

                var ex = Assert.Throws<RuntimeException>(() => dependencyResolver.ResolveDependency("service", typeof(IService), "${componentId}"));
                Assert.AreEqual(string.Format("Could not inject component with id 'componentId' into a dependency of type '{0}' because it is of the wrong type even though the component was explicitly specified using the '${{component.id}}' property value syntax.",
                    typeof(IService)), ex.Message);
                serviceLocator.VerifyAllExpectations();
                resourceLocator.VerifyAllExpectations();
            }

            [Test]
            public void ResolveDependency_WhenDependencyIsOfArrayType_SplitsAndParsesPropertyStringComponentsIndependently()
            {
                var serviceLocator = MockRepository.GenerateMock<IServiceLocator>();
                var resourceLocator = MockRepository.GenerateMock<IResourceLocator>();
                var dependencyResolver = new DefaultObjectDependencyResolver(serviceLocator, resourceLocator);

                var result = dependencyResolver.ResolveDependency("array", typeof(string[]), "abc;def;ghi");

                Assert.Multiple(() =>
                {
                    Assert.IsTrue(result.IsSatisfied);
                    Assert.IsInstanceOfType<string[]>(result.Value);
                    Assert.AreEqual(new[] { "abc", "def", "ghi" }, result.Value);
                });

                serviceLocator.VerifyAllExpectations();
                resourceLocator.VerifyAllExpectations();
            }

            [Test]
            public void ResolveDependency_WhenDependencyIsOfEnumType_ParsesPropertyStringToEnumValueCaseInsensitively()
            {
                var serviceLocator = MockRepository.GenerateMock<IServiceLocator>();
                var resourceLocator = MockRepository.GenerateMock<IResourceLocator>();
                var dependencyResolver = new DefaultObjectDependencyResolver(serviceLocator, resourceLocator);

                var result = dependencyResolver.ResolveDependency("enum", typeof(YesNo), "no");

                Assert.Multiple(() =>
                {
                    Assert.IsTrue(result.IsSatisfied);
                    Assert.IsInstanceOfType<YesNo>(result.Value);
                    Assert.AreEqual(YesNo.No, result.Value);
                });

                serviceLocator.VerifyAllExpectations();
                resourceLocator.VerifyAllExpectations();
            }

            [Test]
            public void ResolveDependency_WhenDependencyIsOfTypeInt_ConvertsPropertyStringToInt()
            {
                var serviceLocator = MockRepository.GenerateMock<IServiceLocator>();
                var resourceLocator = MockRepository.GenerateMock<IResourceLocator>();
                var dependencyResolver = new DefaultObjectDependencyResolver(serviceLocator, resourceLocator);

                var result = dependencyResolver.ResolveDependency("int", typeof(int), "42");

                Assert.Multiple(() =>
                {
                    Assert.IsTrue(result.IsSatisfied);
                    Assert.IsInstanceOfType<int>(result.Value);
                    Assert.AreEqual(42, result.Value);
                });

                serviceLocator.VerifyAllExpectations();
                resourceLocator.VerifyAllExpectations();
            }

            [Test]
            public void ResolveDependency_WhenDependencyIsOfTypeImage_LoadsImageFromResource()
            {
                var serviceLocator = MockRepository.GenerateMock<IServiceLocator>();
                var resourceLocator = MockRepository.GenerateMock<IResourceLocator>();
                var dependencyResolver = new DefaultObjectDependencyResolver(serviceLocator, resourceLocator);
                resourceLocator.Expect(x => x.GetFullPath("SampleImage.png")).Return(@"..\Resources\SampleImage.png");

                var result = dependencyResolver.ResolveDependency("image", typeof(Image), "SampleImage.png");

                Assert.Multiple(() =>
                {
                    Assert.IsTrue(result.IsSatisfied);
                    Assert.IsInstanceOfType<Image>(result.Value);
                });

                serviceLocator.VerifyAllExpectations();
                resourceLocator.VerifyAllExpectations();
            }

            [Test]
            public void ResolveDependency_WhenDependencyIsOfTypeIcon_LoadsIconFromResource()
            {
                var serviceLocator = MockRepository.GenerateMock<IServiceLocator>();
                var resourceLocator = MockRepository.GenerateMock<IResourceLocator>();
                var dependencyResolver = new DefaultObjectDependencyResolver(serviceLocator, resourceLocator);
                resourceLocator.Expect(x => x.GetFullPath("SampleIcon.ico")).Return(@"..\Resources\SampleIcon.ico");

                var result = dependencyResolver.ResolveDependency("icon", typeof(Icon), "SampleIcon.ico");

                Assert.Multiple(() =>
                {
                    Assert.IsTrue(result.IsSatisfied);
                    Assert.IsInstanceOfType<Icon>(result.Value);
                });

                serviceLocator.VerifyAllExpectations();
                resourceLocator.VerifyAllExpectations();
            }

            [Test]
            public void ResolveDependency_WhenDependencyIsOfTypeFileInfo_CreatesFileInfoForResource()
            {
                var serviceLocator = MockRepository.GenerateMock<IServiceLocator>();
                var resourceLocator = MockRepository.GenerateMock<IResourceLocator>();
                var dependencyResolver = new DefaultObjectDependencyResolver(serviceLocator, resourceLocator);
                resourceLocator.Expect(x => x.GetFullPath("file.txt")).Return(@"C:\file.txt");

                var result = dependencyResolver.ResolveDependency("fileInfo", typeof(FileInfo), "file.txt");

                Assert.Multiple(() =>
                {
                    Assert.IsTrue(result.IsSatisfied);
                    Assert.IsInstanceOfType<FileInfo>(result.Value);
                    Assert.AreEqual(@"C:\file.txt", result.Value.ToString());
                });

                serviceLocator.VerifyAllExpectations();
                resourceLocator.VerifyAllExpectations();
            }

            [Test]
            public void ResolveDependency_WhenDependencyIsOfTypeDirectoryInfo_CreatesDirectoryInfoForResource()
            {
                var serviceLocator = MockRepository.GenerateMock<IServiceLocator>();
                var resourceLocator = MockRepository.GenerateMock<IResourceLocator>();
                var dependencyResolver = new DefaultObjectDependencyResolver(serviceLocator, resourceLocator);
                resourceLocator.Expect(x => x.GetFullPath("dir")).Return(@"C:\dir");

                var result = dependencyResolver.ResolveDependency("directoryInfo", typeof(DirectoryInfo), "dir");

                Assert.Multiple(() =>
                {
                    Assert.IsTrue(result.IsSatisfied);
                    Assert.IsInstanceOfType<DirectoryInfo>(result.Value);
                    Assert.AreEqual(@"C:\dir", result.Value.ToString());
                });

                serviceLocator.VerifyAllExpectations();
                resourceLocator.VerifyAllExpectations();
            }
        }

        public interface IService
        {
        }

        public enum YesNo
        {
            Yes, No
        }

        private static ComponentHandle<TService, TTraits> CreateStubComponentHandle<TService, TTraits>()
            where TTraits : Traits
        {
            var serviceDescriptor = MockRepository.GenerateMock<IServiceDescriptor>();
            serviceDescriptor.Stub(x => x.ResolveServiceType()).Return(typeof(TService));
            serviceDescriptor.Stub(x => x.ResolveTraitsType()).Return(typeof(TTraits));

            var componentDescriptor = MockRepository.GenerateMock<IComponentDescriptor>();
            componentDescriptor.Stub(x => x.Service).Return(serviceDescriptor);
            return ComponentHandle.CreateInstance<TService, TTraits>(componentDescriptor);
        }
    }
}