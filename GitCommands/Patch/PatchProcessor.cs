        public Encoding FilesContentEncoding { get; private set; }

        public PatchProcessor(Encoding filesContentEncoding)
        {
            FilesContentEncoding = filesContentEncoding;
        }

        private enum PatchProcessorState
        {
            InHeader,
            InBody,
            OutsidePatch
        }

        /// <summary>
        /// Diff part of patch is printed verbatim, everything else (header, warnings, ...) is printed in git encoding (Settings.SystemEncoding) 
        /// Since patch may contain diff for more than one file, it would be nice to obtaining encoding for each of file
        /// from .gitattributes, for now there is used one encoding, common for every file in repo (Settings.FilesEncoding)
        /// File path can be quoted see core.quotepath, it is unquoted by GitCommandHelpers.ReEncodeFileNameFromLossless
        /// </summary>
        /// <param name="textReader"></param>
        /// <returns></returns>
            bool validate;
            PatchProcessorState state = PatchProcessorState.OutsidePatch;

                validate = true;
                    state = PatchProcessorState.InHeader;
                    validate = false;
                    input = GitCommandHelpers.ReEncodeFileNameFromLossless(input);
                    patch.PatchHeader = input;
                    patch.Type = Patch.PatchType.ChangeFile;
                    ExtractPatchFilenames(patch);
                }
                else if (state == PatchProcessorState.InHeader)
                {
                    if (IsChunkHeader(input))
                        state = PatchProcessorState.InBody;
                    else
                        //header lines are encoded in Settings.SystemEncoding
                        input = GitCommandHelpers.ReEncodeStringFromLossless(input, Settings.SystemEncoding);
                        if (IsIndexLine(input))
                        {                            
                            validate = false;
                        else
                            if (SetPatchType(input, patch))
                            { }
                            else if (IsUnlistedBinaryFileDelete(input))
                            {
                                if (patch.Type != Patch.PatchType.DeleteFile)
                                    throw new FormatException("Change not parsed correct: " + input);
                                patch.File = Patch.FileType.Binary;
                                patch = null;
                                state = PatchProcessorState.OutsidePatch;
                            }
                            else if (IsUnlistedBinaryNewFile(input))
                            {
                                if (patch.Type != Patch.PatchType.NewFile)
                                    throw new FormatException("Change not parsed correct: " + input);

                                patch.File = Patch.FileType.Binary;
                                //TODO: NOT SUPPORTED!
                                patch.Apply = false;
                                patch = null;
                                state = PatchProcessorState.OutsidePatch;
                            }
                            else if (IsBinaryPatch(input))
                            {
                                patch.File = Patch.FileType.Binary;
                                //TODO: NOT SUPPORTED!
                                patch.Apply = false;
                                patch = null;
                                state = PatchProcessorState.OutsidePatch;
                            }
                if (state != PatchProcessorState.OutsidePatch)
                    if (validate)
                        ValidateInput(ref input, patch, state);
                }
        private void ValidateInput(ref string input, Patch patch, PatchProcessorState state)
            if (state == PatchProcessorState.InHeader)
                //--- /dev/null
                //means there is no old file, so this should be a new file
                if (IsOldFileMissing(input))
                {
                    if (patch.Type != Patch.PatchType.NewFile)
                        throw new FormatException("Change not parsed correct: " + input);
                }
                //line starts with --- means, old file name
                else if (input.StartsWith("--- "))
                {
                    input = GitCommandHelpers.UnquoteFileName(input);
                    Match regexMatch = Regex.Match(input, "[-]{3}[ ][\\\"]{0,1}[a]/(.*)[\\\"]{0,1}");
                    if (!regexMatch.Success || patch.FileNameA != (regexMatch.Groups[1].Value.Trim()))
                        throw new FormatException("Old filename not parsed correct: " + input);
                }
                else if (IsNewFileMissing(input))
                {
                    if (patch.Type != Patch.PatchType.DeleteFile)
                        throw new FormatException("Change not parsed correct: " + input);
                }
                //line starts with +++ means, new file name
                //we expect a new file now!
                else if (input.StartsWith("+++ "))
                {
                    input = GitCommandHelpers.UnquoteFileName(input);
                    Match regexMatch = Regex.Match(input, "[+]{3}[ ][\\\"]{0,1}[b]/(.*)[\\\"]{0,1}");
                    if (!regexMatch.Success || patch.FileNameB != (regexMatch.Groups[1].Value.Trim()))
                        throw new FormatException("New filename not parsed correct: " + input);
                }
            else
            {
                input = GitCommandHelpers.ReEncodeStringFromLossless(input, FilesContentEncoding);
                if (!input.StartsWithAny(new string[] { " ", "-", "+", "@", "\\" }))
                    throw new FormatException("Line starts with unexpected character: " + input);
            }               
        private static bool IsChunkHeader(string input)
            return input.StartsWith("@@");
        private static bool IsNewFileMissing(string input)
            return input.StartsWith("+++ /dev/null");
        private static void ExtractPatchFilenames(Patch patch)
            Match match = Regex.Match(patch.PatchHeader,
        private static bool SetPatchType(string input, Patch patch)
            else if (input.StartsWith("old mode "))
                patch.Type = Patch.PatchType.ChangeFileMode;
                return false;

            return true;