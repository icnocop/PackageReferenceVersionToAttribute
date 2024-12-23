﻿// <copyright file="MockMonitorSelection.cs" company="Rami Abughazaleh">
//   Copyright (c) Rami Abughazaleh. All rights reserved.
// </copyright>

namespace PackageReferenceVersionToAttributeExtensionTests.Mocks
{
    using System;
    using System.Runtime.InteropServices;
    using Microsoft.Extensions.Logging;
    using Microsoft.VisualStudio;
    using Microsoft.VisualStudio.Shell.Interop;
    using PackageReferenceVersionToAttributeExtensionTests.Logging;

    /// <summary>
    /// Mock monitor selection.
    /// </summary>
    internal class MockMonitorSelection : IVsMonitorSelection
    {
        private readonly MockMultiItemSelect multiItemSelect;
        private readonly MockHierarchy hierarchy;
        private readonly ILogger<MockMonitorSelection> logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="MockMonitorSelection"/> class.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="multiItemSelect">The multi-item select.</param>
        /// <param name="hierarchy">The hierarchy.</param>
        public MockMonitorSelection(
            ILogger<MockMonitorSelection> logger,
            MockMultiItemSelect multiItemSelect,
            MockHierarchy hierarchy)
        {
            this.logger = logger;
            this.multiItemSelect = multiItemSelect;
            this.hierarchy = hierarchy;
        }

        /// <inheritdoc/>
        public int GetCurrentSelection(out IntPtr ppHier, out uint pitemid, out IVsMultiItemSelect ppMIS, out IntPtr ppSC)
        {
            ppMIS = this.multiItemSelect;
            ppSC = IntPtr.Zero;

            if (this.hierarchy != null)
            {
                ppHier = Marshal.GetIUnknownForObject(this.hierarchy);
                if (this.multiItemSelect == null)
                {
                    pitemid = 0;
                }
                else
                {
                    pitemid = VSConstants.VSITEMID_SELECTION;
                }
            }
            else
            {
                ppHier = IntPtr.Zero;
                pitemid = 0;
            }

            this.logger.LogDebug(
                $"""
                GetCurrentSelection called:
                    ppHier = {ppHier}
                    pitemid = {ItemIdFormatter.Format(pitemid)}
                    ppMIS = {this.multiItemSelect.ToString(1)}
                    ppSC = {ppSC}
                """);

            return VSConstants.S_OK;
        }

        /// <inheritdoc/>
        public int AdviseSelectionEvents(IVsSelectionEvents pSink, out uint pdwCookie)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public int UnadviseSelectionEvents(uint dwCookie)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public int GetCurrentElementValue(uint elementid, out object pvarValue)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public int GetCmdUIContextCookie(ref Guid rguidCmdUI, out uint pdwCmdUICookie)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public int IsCmdUIContextActive(uint dwCmdUICookie, out int pfActive)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public int SetCmdUIContext(uint dwCmdUICookie, int fActive)
        {
            throw new NotImplementedException();
        }
    }
}