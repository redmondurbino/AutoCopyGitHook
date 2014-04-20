AutoCopyGitHook
=================

Unity utility to automatically copy the a git hook file (defaults to "pre-commit") into the user's local .git/hooks directory.

Make sure the source file has the executable flag set.
Change as needed these constants in CopyGitHooksPostprocessor.cs: dotGitRelativeDirectoryPath, hookRelativeDirectoryPath, and hookFilename.
