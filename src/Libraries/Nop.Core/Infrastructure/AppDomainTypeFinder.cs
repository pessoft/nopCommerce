using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace Nop.Core.Infrastructure
{
    /// <summary>
    /// A class that finds types needed by Nop by looping assemblies in the currently executing AppDomain. Only assemblies whose names matches
    /// certain patterns are investigated and an optional list of assemblies referenced by <see cref="AssemblyNames"/> are always investigated.
    /// </summary>
    public partial class AppDomainTypeFinder : ITypeFinder
    {
        #region Fields

        private readonly bool _ignoreReflectionErrors = true;

        protected INopFileProvider _fileProvider;

        #endregion

        #region Ctor

        public AppDomainTypeFinder(INopFileProvider fileProvider = null)
        {
            this._fileProvider = fileProvider ?? CommonHelper.DefaultFileProvider;
        }

        #endregion

        #region Utilities

        /// <summary>
        /// Iterates all assemblies in the AppDomain and if it's name matches the configured patterns add it to our list.
        /// </summary>
        /// <param name="addedAssemblyNames">Added assembly names</param>
        /// <param name="assemblies">Assemblies</param>
        private void AddAssembliesInAppDomain(List<string> addedAssemblyNames, List<Assembly> assemblies)
        {
            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                if (!Matches(assembly.FullName))
                    continue;

                if (addedAssemblyNames.Contains(assembly.FullName))
                    continue;

                assemblies.Add(assembly);
                addedAssemblyNames.Add(assembly.FullName);
            }
        }

        /// <summary>
        /// Iterates all assemblies in the AppDomain and if it's name matches the configured patterns add it to our list.
        /// </summary>
        /// <param name="addedAssemblyNames">Added assembly names</param>
        /// <param name="assemblies">Assemblies</param>
        /// <param name="cancellationToken">A cancellation token to observe while waiting for the task to complete</param>
        /// <returns>The asynchronous task whose result determines that assemblies added</returns>
        protected virtual async Task AddAssembliesInAppDomainAsync(List<string> addedAssemblyNames, List<Assembly> assemblies,
            CancellationToken cancellationToken)
        {
            await Task.Run(async () =>
            {
                foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
                {
                    if (!(await MatchesAsync(assembly.FullName, cancellationToken)))
                        continue;

                    if (addedAssemblyNames.Contains(assembly.FullName))
                        continue;

                    assemblies.Add(assembly);
                    addedAssemblyNames.Add(assembly.FullName);
                }
            }, cancellationToken);
        }

        /// <summary>
        /// Adds specifically configured assemblies.
        /// </summary>
        /// <param name="addedAssemblyNames">Added assembly names</param>
        /// <param name="assemblies">Assemblies</param>
        protected virtual void AddConfiguredAssemblies(List<string> addedAssemblyNames, List<Assembly> assemblies)
        {
            foreach (var assemblyName in AssemblyNames)
            {
                var assembly = Assembly.Load(assemblyName);
                if (addedAssemblyNames.Contains(assembly.FullName))
                    continue;

                assemblies.Add(assembly);
                addedAssemblyNames.Add(assembly.FullName);
            }
        }

        /// <summary>
        /// Adds specifically configured assemblies.
        /// </summary>
        /// <param name="addedAssemblyNames">Added assembly names</param>
        /// <param name="assemblies">Assemblies</param>
        /// <param name="cancellationToken">A cancellation token to observe while waiting for the task to complete</param>
        /// <returns>The asynchronous task whose result determines that assemblies added</returns>
        protected virtual async Task AddConfiguredAssembliesAsync(List<string> addedAssemblyNames, List<Assembly> assemblies,
            CancellationToken cancellationToken)
        {
            await Task.Run(() =>
            {
                foreach (var assemblyName in AssemblyNames)
                {
                    var assembly = Assembly.Load(assemblyName);
                    if (addedAssemblyNames.Contains(assembly.FullName))
                        continue;

                    assemblies.Add(assembly);
                    addedAssemblyNames.Add(assembly.FullName);
                }
            }, cancellationToken);
        }

        /// <summary>
        /// Check if a dll is one of the shipped dlls that we know don't need to be investigated.
        /// </summary>
        /// <param name="assemblyFullName">
        /// The name of the assembly to check.
        /// </param>
        /// <returns>
        /// True if the assembly should be loaded into Nop.
        /// </returns>
        protected virtual bool Matches(string assemblyFullName)
        {
            return !Matches(assemblyFullName, AssemblySkipLoadingPattern)
                   && Matches(assemblyFullName, AssemblyRestrictToLoadingPattern);
        }

        /// <summary>
        /// Check if a dll is one of the shipped dlls that we know don't need to be investigated.
        /// </summary>
        /// <param name="assemblyFullName">The name of the assembly to check.</param>
        /// <param name="cancellationToken">A cancellation token to observe while waiting for the task to complete</param>
        /// <returns>The asynchronous task whose result determines whether the assembly should be loaded</returns>
        protected virtual async Task<bool> MatchesAsync(string assemblyFullName, CancellationToken cancellationToken)
        {
            return !(await MatchesAsync(assemblyFullName, AssemblySkipLoadingPattern, cancellationToken))
                && await MatchesAsync(assemblyFullName, AssemblyRestrictToLoadingPattern, cancellationToken);
        }

        /// <summary>
        /// Check if a dll is one of the shipped dlls that we know don't need to be investigated.
        /// </summary>
        /// <param name="assemblyFullName">
        /// The assembly name to match.
        /// </param>
        /// <param name="pattern">
        /// The regular expression pattern to match against the assembly name.
        /// </param>
        /// <returns>
        /// True if the pattern matches the assembly name.
        /// </returns>
        protected virtual bool Matches(string assemblyFullName, string pattern)
        {
            return Regex.IsMatch(assemblyFullName, pattern, RegexOptions.IgnoreCase | RegexOptions.Compiled);
        }

        /// <summary>
        /// Check if a dll is one of the shipped dlls that we know don't need to be investigated.
        /// </summary>
        /// <param name="assemblyFullName">The assembly name to match.</param>
        /// <param name="pattern">The regular expression pattern to match against the assembly name.</param>
        /// <param name="cancellationToken">A cancellation token to observe while waiting for the task to complete</param>
        /// <returns>The asynchronous task whose result determines whether the assembly should be loaded</returns>
        protected virtual async Task<bool> MatchesAsync(string assemblyFullName, string pattern, CancellationToken cancellationToken)
        {
            return await Task.Run(() =>
                Regex.IsMatch(assemblyFullName, pattern, RegexOptions.IgnoreCase | RegexOptions.Compiled), cancellationToken);
        }

        /// <summary>
        /// Makes sure matching assemblies in the supplied folder are loaded in the app domain.
        /// </summary>
        /// <param name="directoryPath">
        /// The physical path to a directory containing dlls to load in the app domain.
        /// </param>
        protected virtual void LoadMatchingAssemblies(string directoryPath)
        {
            var loadedAssemblyNames = new List<string>();

            foreach (var a in GetAssemblies())
            {
                loadedAssemblyNames.Add(a.FullName);
            }

            if (!_fileProvider.DirectoryExists(directoryPath))
            {
                return;
            }

            foreach (var dllPath in _fileProvider.GetFiles(directoryPath, "*.dll"))
            {
                try
                {
                    var an = AssemblyName.GetAssemblyName(dllPath);
                    if (Matches(an.FullName) && !loadedAssemblyNames.Contains(an.FullName))
                    {
                        App.Load(an);
                    }

                    //old loading stuff
                    //Assembly a = Assembly.ReflectionOnlyLoadFrom(dllPath);
                    //if (Matches(a.FullName) && !loadedAssemblyNames.Contains(a.FullName))
                    //{
                    //    App.Load(a.FullName);
                    //}
                }
                catch (BadImageFormatException ex)
                {
                    Trace.TraceError(ex.ToString());
                }
            }
        }

        /// <summary>
        /// Makes sure matching assemblies in the supplied folder are loaded in the app domain.
        /// </summary>
        /// <param name="directoryPath">The physical path to a directory containing dlls to load in the app domain.</param>
        /// <param name="cancellationToken">A cancellation token to observe while waiting for the task to complete</param>
        /// <returns>The asynchronous task whose result determines that assemblies are loaded</returns>
        protected virtual async Task LoadMatchingAssembliesAsync(string directoryPath, CancellationToken cancellationToken)
        {
            var loadedAssemblyNames = new List<string>();

            foreach (var assembly in await GetAssembliesAsync(cancellationToken))
            {
                loadedAssemblyNames.Add(assembly.FullName);
            }

            if (!_fileProvider.DirectoryExists(directoryPath))
                return;

            foreach (var dllPath in _fileProvider.GetFiles(directoryPath, "*.dll"))
            {
                try
                {
                    var assemblyName = AssemblyName.GetAssemblyName(dllPath);
                    if (await MatchesAsync(assemblyName.FullName, cancellationToken) && !loadedAssemblyNames.Contains(assemblyName.FullName))
                    {
                        App.Load(assemblyName);
                    }
                }
                catch (BadImageFormatException)
                {
                }
            }
        }

        /// <summary>
        /// Does type implement generic?
        /// </summary>
        /// <param name="type"></param>
        /// <param name="openGeneric"></param>
        /// <returns></returns>
        protected virtual bool DoesTypeImplementOpenGeneric(Type type, Type openGeneric)
        {
            try
            {
                var genericTypeDefinition = openGeneric.GetGenericTypeDefinition();
                foreach (var implementedInterface in type.FindInterfaces((objType, objCriteria) => true, null))
                {
                    if (!implementedInterface.IsGenericType)
                        continue;

                    var isMatch = genericTypeDefinition.IsAssignableFrom(implementedInterface.GetGenericTypeDefinition());
                    return isMatch;
                }

                return false;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Does type implement generic?
        /// </summary>
        /// <param name="type"></param>
        /// <param name="openGeneric"></param>
        /// <param name="cancellationToken">A cancellation token to observe while waiting for the task to complete</param>
        /// <returns>The asynchronous task whose result determines whether type implements generic</returns>
        protected virtual async Task<bool> DoesTypeImplementOpenGenericAsync(Type type, Type openGeneric, CancellationToken cancellationToken)
        {
            return await Task.Run(() =>
            {
                try
                {
                    var genericTypeDefinition = openGeneric.GetGenericTypeDefinition();
                    foreach (var implementedInterface in type.FindInterfaces((objType, objCriteria) => true, null))
                    {
                        if (!implementedInterface.IsGenericType)
                            continue;

                        var isMatch = genericTypeDefinition.IsAssignableFrom(implementedInterface.GetGenericTypeDefinition());
                        return isMatch;
                    }

                    return false;
                }
                catch
                {
                    return false;
                }
            }, cancellationToken);
        }

        #endregion

        #region Methods

        /// <summary>
        /// Find classes of type
        /// </summary>
        /// <typeparam name="T">Type</typeparam>
        /// <param name="onlyConcreteClasses">A value indicating whether to find only concrete classes</param>
        /// <returns>Result</returns>
        public virtual IEnumerable<Type> FindClassesOfType<T>(bool onlyConcreteClasses = true)
        {
            return FindClassesOfType(typeof(T), onlyConcreteClasses);
        }

        /// <summary>
        /// Find classes of type
        /// </summary>
        /// <typeparam name="T">Type</typeparam>
        /// <param name="onlyConcreteClasses">A value indicating whether to find only concrete classes</param>
        /// <param name="cancellationToken">A cancellation token to observe while waiting for the task to complete</param>
        /// <returns>The asynchronous task whose result contains classes of passed type</returns>
        public virtual async Task<IEnumerable<Type>> FindClassesOfTypeAsync<T>(bool onlyConcreteClasses = true,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            return await FindClassesOfTypeAsync(typeof(T), onlyConcreteClasses, cancellationToken);
        }

        /// <summary>
        /// Find classes of type
        /// </summary>
        /// <param name="assignTypeFrom">Assign type from</param>
        /// <param name="onlyConcreteClasses">A value indicating whether to find only concrete classes</param>
        /// <returns>Result</returns>
        public virtual IEnumerable<Type> FindClassesOfType(Type assignTypeFrom, bool onlyConcreteClasses = true)
        {
            return FindClassesOfType(assignTypeFrom, GetAssemblies(), onlyConcreteClasses);
        }

        /// <summary>
        /// Find classes of type
        /// </summary>
        /// <param name="assignTypeFrom">Assign type from</param>
        /// <param name="onlyConcreteClasses">A value indicating whether to find only concrete classes</param>
        /// <param name="cancellationToken">A cancellation token to observe while waiting for the task to complete</param>
        /// <returns>The asynchronous task whose result contains classes of passed type</returns>
        public virtual async Task<IEnumerable<Type>> FindClassesOfTypeAsync(Type assignTypeFrom, bool onlyConcreteClasses = true,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            return await FindClassesOfTypeAsync(assignTypeFrom, await GetAssembliesAsync(cancellationToken), onlyConcreteClasses, cancellationToken);
        }

        /// <summary>
        /// Find classes of type
        /// </summary>
        /// <typeparam name="T">Type</typeparam>
        /// <param name="assemblies">Assemblies</param>
        /// <param name="onlyConcreteClasses">A value indicating whether to find only concrete classes</param>
        /// <returns>Result</returns>
        public virtual IEnumerable<Type> FindClassesOfType<T>(IEnumerable<Assembly> assemblies, bool onlyConcreteClasses = true)
        {
            return FindClassesOfType(typeof(T), assemblies, onlyConcreteClasses);
        }

        /// <summary>
        /// Find classes of type
        /// </summary>
        /// <typeparam name="T">Type</typeparam>
        /// <param name="assemblies">Assemblies</param>
        /// <param name="onlyConcreteClasses">A value indicating whether to find only concrete classes</param>
        /// <param name="cancellationToken">A cancellation token to observe while waiting for the task to complete</param>
        /// <returns>The asynchronous task whose result contains classes of passed type</returns>
        public virtual async Task<IEnumerable<Type>> FindClassesOfTypeAsync<T>(IEnumerable<Assembly> assemblies, bool onlyConcreteClasses = true,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            return await FindClassesOfTypeAsync(typeof(T), assemblies, onlyConcreteClasses, cancellationToken);
        }

        /// <summary>
        /// Find classes of type
        /// </summary>
        /// <param name="assignTypeFrom">Assign type from</param>
        /// <param name="assemblies">Assemblies</param>
        /// <param name="onlyConcreteClasses">A value indicating whether to find only concrete classes</param>
        /// <returns>Result</returns>
        public virtual IEnumerable<Type> FindClassesOfType(Type assignTypeFrom, IEnumerable<Assembly> assemblies, bool onlyConcreteClasses = true)
        {
            var result = new List<Type>();
            try
            {
                foreach (var a in assemblies)
                {
                    Type[] types = null;
                    try
                    {
                        types = a.GetTypes();
                    }
                    catch
                    {
                        //Entity Framework 6 doesn't allow getting types (throws an exception)
                        if (!_ignoreReflectionErrors)
                        {
                            throw;
                        }
                    }

                    if (types == null)
                        continue;

                    foreach (var t in types)
                    {
                        if (!assignTypeFrom.IsAssignableFrom(t) && (!assignTypeFrom.IsGenericTypeDefinition || !DoesTypeImplementOpenGeneric(t, assignTypeFrom)))
                            continue;

                        if (t.IsInterface)
                            continue;

                        if (onlyConcreteClasses)
                        {
                            if (t.IsClass && !t.IsAbstract)
                            {
                                result.Add(t);
                            }
                        }
                        else
                        {
                            result.Add(t);
                        }
                    }
                }
            }
            catch (ReflectionTypeLoadException ex)
            {
                var msg = string.Empty;
                foreach (var e in ex.LoaderExceptions)
                    msg += e.Message + Environment.NewLine;

                var fail = new Exception(msg, ex);
                Debug.WriteLine(fail.Message, fail);

                throw fail;
            }

            return result;
        }

        /// <summary>
        /// Find classes of type
        /// </summary>
        /// <param name="assignTypeFrom">Assign type from</param>
        /// <param name="assemblies">Assemblies</param>
        /// <param name="onlyConcreteClasses">A value indicating whether to find only concrete classes</param>
        /// <param name="cancellationToken">A cancellation token to observe while waiting for the task to complete</param>
        /// <returns>The asynchronous task whose result contains classes of passed type</returns>
        public virtual async Task<IEnumerable<Type>> FindClassesOfTypeAsync(Type assignTypeFrom, IEnumerable<Assembly> assemblies,
            bool onlyConcreteClasses = true, CancellationToken cancellationToken = default(CancellationToken))
        {
            return await Task.Run(async () =>
            {
                var result = new List<Type>();
                try
                {
                    foreach (var a in assemblies)
                    {
                        Type[] types = null;
                        try
                        {
                            types = a.GetTypes();
                        }
                        catch
                        {
                            //Entity Framework 6 doesn't allow getting types (throws an exception)
                            if (!_ignoreReflectionErrors)
                            {
                                throw;
                            }
                        }

                        if (types == null)
                            continue;

                        foreach (var t in types)
                        {
                            if (!assignTypeFrom.IsAssignableFrom(t) && (!assignTypeFrom.IsGenericTypeDefinition || !(await DoesTypeImplementOpenGenericAsync(t, assignTypeFrom, cancellationToken))))
                                continue;

                            if (t.IsInterface)
                                continue;

                            if (onlyConcreteClasses)
                            {
                                if (t.IsClass && !t.IsAbstract)
                                {
                                    result.Add(t);
                                }
                            }
                            else
                            {
                                result.Add(t);
                            }
                        }
                    }
                }
                catch (ReflectionTypeLoadException ex)
                {
                    var msg = string.Empty;
                    foreach (var e in ex.LoaderExceptions)
                        msg += e.Message + Environment.NewLine;

                    var fail = new Exception(msg, ex);
                    Debug.WriteLine(fail.Message, fail);

                    throw fail;
                }

                return result;
            }, cancellationToken);
        }

        /// <summary>
        /// Gets the assemblies related to the current implementation.
        /// </summary>
        /// <returns>A list of assemblies</returns>
        public virtual IList<Assembly> GetAssemblies()
        {
            var addedAssemblyNames = new List<string>();
            var assemblies = new List<Assembly>();

            if (LoadAppDomainAssemblies)
                AddAssembliesInAppDomain(addedAssemblyNames, assemblies);
            AddConfiguredAssemblies(addedAssemblyNames, assemblies);

            return assemblies;
        }

        /// <summary>
        /// Gets the assemblies related to the current implementation.
        /// </summary>
        /// <param name="cancellationToken">A cancellation token to observe while waiting for the task to complete</param>
        /// <returns>The asynchronous task whose result contains a list of assemblies</returns>
        public virtual async Task<IList<Assembly>> GetAssembliesAsync(CancellationToken cancellationToken)
        {
            var addedAssemblyNames = new List<string>();
            var assemblies = new List<Assembly>();

            if (LoadAppDomainAssemblies)
                await AddAssembliesInAppDomainAsync(addedAssemblyNames, assemblies, cancellationToken);

            await AddConfiguredAssembliesAsync(addedAssemblyNames, assemblies, cancellationToken);

            return assemblies;
        }

        #endregion

        #region Properties

        /// <summary>The app domain to look for types in.</summary>
        public virtual AppDomain App => AppDomain.CurrentDomain;

        /// <summary>Gets or sets whether Nop should iterate assemblies in the app domain when loading Nop types. Loading patterns are applied when loading these assemblies.</summary>
        public bool LoadAppDomainAssemblies { get; set; } = true;

        /// <summary>Gets or sets assemblies loaded a startup in addition to those loaded in the AppDomain.</summary>
        public IList<string> AssemblyNames { get; set; } = new List<string>();

        /// <summary>Gets the pattern for dlls that we know don't need to be investigated.</summary>
        public string AssemblySkipLoadingPattern { get; set; } = "^System|^mscorlib|^Microsoft|^AjaxControlToolkit|^Antlr3|^Autofac|^AutoMapper|^Castle|^ComponentArt|^CppCodeProvider|^DotNetOpenAuth|^EntityFramework|^EPPlus|^FluentValidation|^ImageResizer|^itextsharp|^log4net|^MaxMind|^MbUnit|^MiniProfiler|^Mono.Math|^MvcContrib|^Newtonsoft|^NHibernate|^nunit|^Org.Mentalis|^PerlRegex|^QuickGraph|^Recaptcha|^Remotion|^RestSharp|^Rhino|^Telerik|^Iesi|^TestDriven|^TestFu|^UserAgentStringLibrary|^VJSharpCodeProvider|^WebActivator|^WebDev|^WebGrease";

        /// <summary>Gets or sets the pattern for dll that will be investigated. For ease of use this defaults to match all but to increase performance you might want to configure a pattern that includes assemblies and your own.</summary>
        /// <remarks>If you change this so that Nop assemblies aren't investigated (e.g. by not including something like "^Nop|..." you may break core functionality.</remarks>
        public string AssemblyRestrictToLoadingPattern { get; set; } = ".*";

        #endregion
    }
}