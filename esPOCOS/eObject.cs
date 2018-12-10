using System;
using System.Reflection;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace eStore.POCOS
{
    class eObject
    {
        public static void Write(Type type)
        {
             
            FieldInfo[] fields = type.GetFields(); // Obtain all fields
            foreach (var field in fields) // Loop through fields
            {
                string name = field.Name; // Get string name
                object temp = field.GetValue(null); // Get value
                if (temp is int) // See if it is an integer.
                {
                    int value = (int)temp;
                    Console.Write(name);
                    Console.Write(" (int) = ");
                    Console.WriteLine(value);
                }
                else if (temp is string) // See if it is a string.
                {
                    string value = temp as string;
                    Console.Write(name);
                    Console.Write(" (string) = ");
                    Console.WriteLine(value);
                }
            }
        }
    }
}
