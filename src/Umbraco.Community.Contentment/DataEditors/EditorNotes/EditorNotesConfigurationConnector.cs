﻿/* Copyright © 2022 Lee Kelleher.
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using System.Collections.Generic;
#if NET472
using Umbraco.Core;
using Umbraco.Core.Deploy;
using Umbraco.Core.Models;
using Umbraco.Core.PropertyEditors;
using Umbraco.Core.Serialization;
using UmbConstants = Umbraco.Core.Constants;
#else
using Umbraco.Cms.Core;
using Umbraco.Cms.Core.Deploy;
using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.PropertyEditors;
using Umbraco.Cms.Core.Serialization;
using Umbraco.Extensions;
using UmbConstants = Umbraco.Cms.Core.Constants;
#endif

namespace Umbraco.Community.Contentment.DataEditors
{
    internal sealed class EditorNotesConfigurationConnector : IDataTypeConfigurationConnector
    {
        private readonly IConfigurationEditorJsonSerializer _configurationEditorJsonSerializer;
        private readonly ILocalLinkParser _localLinkParser;
        private readonly IImageSourceParser _imageSourceParser;
        private readonly IMacroParser _macroParser;

        public IEnumerable<string> PropertyEditorAliases => new[] { EditorNotesDataEditor.DataEditorName };

        public EditorNotesConfigurationConnector(
            IConfigurationEditorJsonSerializer configurationEditorJsonSerializer,
            ILocalLinkParser localLinkParser,
            IImageSourceParser imageSourceParser,
            IMacroParser macroParser)
        {
            _configurationEditorJsonSerializer = configurationEditorJsonSerializer;
            _localLinkParser = localLinkParser;
            _imageSourceParser = imageSourceParser;
            _macroParser = macroParser;
        }

#if NET8_0_OR_GREATER
        public object FromArtifact(IDataType dataType, string configuration, IContextCache contextCache)
#else
        public object FromArtifact(IDataType dataType, string configuration)
#endif
        {
            var dataTypeConfigurationEditor = dataType.Editor.GetConfigurationEditor();

            var db = dataTypeConfigurationEditor.FromDatabase(configuration, _configurationEditorJsonSerializer);

            if (db is Dictionary<string, object> config &&
                config.TryGetValueAs(EditorNotesConfigurationEditor.Message, out string notes) == true &&
                string.IsNullOrWhiteSpace(notes) == false)
            {
                notes = _localLinkParser.FromArtifact(notes);
                notes = _imageSourceParser.FromArtifact(notes);
                notes = _macroParser.FromArtifact(notes);

                config[EditorNotesConfigurationEditor.Message] = notes;

                return config;
            }

            return db;
        }

#if NET8_0_OR_GREATER
        public string ToArtifact(IDataType dataType, ICollection<ArtifactDependency> dependencies, IContextCache contextCache)
#else
        public string ToArtifact(IDataType dataType, ICollection<ArtifactDependency> dependencies)
#endif
        {
            if (dataType.Configuration is Dictionary<string, object> config &&
                config.TryGetValueAs(EditorNotesConfigurationEditor.Message, out string notes) == true &&
                string.IsNullOrWhiteSpace(notes) == false)
            {
                var udis = new List<Udi>();

                notes = _localLinkParser.ToArtifact(notes, udis);
                notes = _imageSourceParser.ToArtifact(notes, udis);
                notes = _macroParser.ToArtifact(notes, udis);

                foreach (var udi in udis)
                {
                    var mode = udi.EntityType == UmbConstants.UdiEntityType.Macro
                        ? ArtifactDependencyMode.Match
                        : ArtifactDependencyMode.Exist;

                    dependencies.Add(new ArtifactDependency(udi, false, mode));
                }

                config[EditorNotesConfigurationEditor.Message] = notes;
            }

#if NET472
            return ConfigurationEditor.ToDatabase(dataType.Configuration);
#else
            return ConfigurationEditor.ToDatabase(dataType.Configuration, _configurationEditorJsonSerializer);
#endif
        }
    }
}
