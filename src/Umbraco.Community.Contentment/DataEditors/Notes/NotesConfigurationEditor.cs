﻿/* Copyright © 2019 Lee Kelleher.
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using System.Collections.Generic;
#if NET472
using Umbraco.Core.IO;
using Umbraco.Core.PropertyEditors;
#else
using Umbraco.Cms.Core.IO;
using Umbraco.Cms.Core.PropertyEditors;
using Umbraco.Extensions;
#endif

namespace Umbraco.Community.Contentment.DataEditors
{
    internal sealed class NotesConfigurationEditor : ConfigurationEditor
    {
        internal const string Notes = NotesConfigurationField.Notes;

        public NotesConfigurationEditor(IIOHelper ioHelper)
            : base()
        {
            Fields.Add(new ConfigurationField
            {
                Key = Notes,
                Name = nameof(Notes),
                Description = "Enter the notes to be displayed for the content editor.",
#if NET8_0_OR_GREATER
                View = ioHelper.ResolveRelativeOrVirtualUrl(RichTextEditorDataEditor.DataEditorViewPath),
#else
                View = ioHelper.ResolveRelativeOrVirtualUrl("~/umbraco/views/propertyeditors/rte/rte.html"),
#endif
                Config = new Dictionary<string, object>
                {
                    { "editor", Constants.Conventions.DefaultConfiguration.RichTextEditor }
                }
            });

            Fields.Add(new HideLabelConfigurationField());
            Fields.Add(new HidePropertyGroupConfigurationField());
        }
    }
}
