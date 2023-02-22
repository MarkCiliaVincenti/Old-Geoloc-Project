using System;
using System.IO;
using System.Linq;
using System.Reflection;

namespace SmartFleet.Core.Infrastructure.MassTransit
{
    public class MicroServicesLoader
    {
        public static void Loader(string path)
        {

            if (String.IsNullOrWhiteSpace(path))
            {
                return;
            }

            //  Gets all compiled assemblies.
            //  This is particularly useful when extending applications functionality from 3rd parties,
            //  if there are interfaces within the modules.
            var assemblies = Directory.GetFiles(path, "*Service.dll", SearchOption.AllDirectories)
                .Select(Assembly.LoadFrom);

            foreach (var assembly in assemblies)
            {
                //  Gets the all modules from each assembly to be registered.
                //  Make sure that each module **MUST** have a parameterless constructor.
                var modules = assembly.GetTypes()
                    .Where(p => typeof(IMicroService).IsAssignableFrom(p)
                                && !p.IsAbstract)
                    .Select(p => (IMicroService)Activator.CreateInstance(p));

                //  Regsiters each module.
                
                foreach (var module in modules)
                {
                    MethodInfo method = module.GetType().GetMethod("StartService");
                    // Debug.WriteLine(module.GetType());
                    if (method != null)
                        method.Invoke(module,null);
                }
            }
        }
    }
}
