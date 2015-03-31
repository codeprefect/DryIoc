﻿using System;
using System.Collections.Generic;
using DryIoc.Web;
using NUnit.Framework;

namespace DryIoc.Mvc.UnitTests
{
    [TestFixture]
    public class ReuseInRequestTests
    {
        [Test]
        public void Working_with_HttpContext_based_scope_context()
        {
            var fakeItems = new Dictionary<object, object>();
            var root = new Container(scopeContext: new HttpContextScopeContext(() => fakeItems));
            root.Register<SomeRoot>(Web.Reuse.InRequest);
            root.Register<SomeDep>(Web.Reuse.InRequest);

            SomeDep savedOutside;
            using (var scoped = root.OpenScope())
            {
                var a = scoped.Resolve<SomeRoot>();
                var b = scoped.Resolve<SomeRoot>();

                Assert.That(a, Is.SameAs(b));

                using (var nested = scoped.OpenScope())
                {
                    var aa = nested.Resolve<SomeRoot>();
                    Assert.AreSame(aa, a);
                }

                savedOutside = a.Dep;
            }

            using (var scoped = root.OpenScope())
            {
                var a = scoped.Resolve<SomeRoot>();
                var b = scoped.Resolve<SomeRoot>();

                Assert.That(a, Is.SameAs(b));
                Assert.That(a.Dep, Is.Not.SameAs(savedOutside));
            }

            Assert.That(savedOutside.IsDisposed, Is.True);
        }

        internal class SomeDep : IDisposable
        {
            public bool IsDisposed { get; private set; }

            public void Dispose()
            {
                IsDisposed = true;
            }
        }

        internal class SomeRoot
        {
            public SomeDep Dep { get; private set; }
            public SomeRoot(SomeDep dep)
            {
                Dep = dep;
            }
        }
    }
}
