using System;
using Moledozer;

namespace DetourTest
{
    public class TestClass
    {
        public virtual void TestMethod()
        {
            Console.Out.WriteLine("TestClass.TestMethod");
        }
    }

    public class TestSubClass : TestClass
    {
    }

    public class OtherSubClass : TestClass
    {
    }

    public class DetourClass : TestClass
    {
        public void DetourMethod()
        {
            if ((this as object).GetType() != typeof(TestSubClass)) {
                TestMethod();
            } else {
                Console.Out.WriteLine("Detour!");
            }
        }

        public void DetourBaseMethod()
        {
            if ((this as object).GetType() != typeof(TestSubClass)) {
                base.TestMethod();
            } else {
                Console.Out.WriteLine("Detour!");
            }
        }
        /*
        public new void TestMethod()
        {
            if ((this as object).GetType() != typeof(TestSubClass)) {
                base.TestMethod();
            } else {
                Console.Out.WriteLine("Detour!");
            }
        }
        */

        public override void TestMethod()
        {
            if ((this as object).GetType() != typeof(TestSubClass)) {
                Console.Out.WriteLine("TestClass.TestMethod");
            } else {
                Console.Out.WriteLine("Detour!");
            }
        }
    }

    public class DetourTest
    {
        static void Main()
        {
            TestClass obj = new TestClass();
            TestSubClass subObj = new TestSubClass();
            OtherSubClass otherObj = new OtherSubClass();

            obj.TestMethod();
            subObj.TestMethod();
            otherObj.TestMethod();

            try {
                RedirectionHelper.RedirectCalls(typeof(TestSubClass).GetMethod("TestMethod"), typeof(DetourClass).GetMethod("TestMethod"));
                obj.TestMethod();
                subObj.TestMethod();
                otherObj.TestMethod();
            } catch (Exception e) {
                Console.Out.WriteLine(e);
            }
        }
    }
}
