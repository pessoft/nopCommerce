using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace Nop.Core.Infrastructure
{
    /// <summary>
    /// Provides information about types in the current web application. 
    /// Optionally this class can look at all assemblies in the bin folder.
    /// </summary>
    public partial class WebAppTypeFinder : AppDomainTypeFinder
    {
        #region Fields

        private bool _binFolderAssembliesLoaded;

        #endregion

        #region Ctor

        public WebAppTypeFinder(INopFileProvider fileProvider = null) : base(fileProvider)
        {
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets whether assemblies in the bin folder of the web application should be specifically checked for being loaded on application load. This is need in situations where plugins need to be loaded in the AppDomain after the application been reloaded.
        /// </summary>
        public bool EnsureBinFolderAssembliesLoaded { get; set; } = true;

        #endregion

        #region Utilities

        /// <summary>
        /// Gets a physical disk path of \Bin directory
        /// </summary>
        /// <returns>The physical path. E.g. "c:\inetpub\wwwroot\bin"</returns>
        protected virtual string GetBinDirectory()
        {
            return AppContext.BaseDirectory;
        }

        /// <summary>
        /// Gets a physical disk path of \Bin directory
        /// </summary>
        /// <param name="cancellationToken">A cancellation token to observe while waiting for the task to complete</param>
        /// <returns>The asynchronous task whose result contains a path to the bin directory</returns>
        protected virtual async Task<string> GetBinDirectoryAsync(CancellationToken cancellationToken)
        {
            return await Task.FromResult(AppContext.BaseDirectory);
        }

        #endregion

        #region Methods

        /// <summary>
        /// Get assemblies
        /// </summary>
        /// <returns>Result</returns>
        public override IList<Assembly> GetAssemblies()
        {
            if (!EnsureBinFolderAssembliesLoaded || _binFolderAssembliesLoaded)
                return base.GetAssemblies();

            _binFolderAssembliesLoaded = true;
            var binPath = GetBinDirectory();
            //binPath = _webHelper.MapPath("~/bin");
            LoadMatchingAssemblies(binPath);

            return base.GetAssemblies();
        }

        /// <summary>
        /// Gets the assemblies related to the current implementation.
        /// </summary>
        /// <param name="cancellationToken">A cancellation token to observe while waiting for the task to complete</param>
        /// <returns>The asynchronous task whose result contains a list of assemblies</returns>
        public override async Task<IList<Assembly>> GetAssembliesAsync(CancellationToken cancellationToken)
        {
            if (!EnsureBinFolderAssembliesLoaded || _binFolderAssembliesLoaded)
                return await base.GetAssembliesAsync(cancellationToken);

            _binFolderAssembliesLoaded = true;
            var binPath = await GetBinDirectoryAsync(cancellationToken);

            await LoadMatchingAssembliesAsync(binPath, cancellationToken);

            return await base.GetAssembliesAsync(cancellationToken);
        }

        #endregion
    }
}