﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Snp.Core.Services;

namespace Snp.Core.ViewModels;

public partial class CollectionsPageViewModel : SamplePageViewModel
{
    public CollectionsPageViewModel(IFilesService filesService) 
        : base(filesService)
    {
    }
}
