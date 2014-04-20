/*
The MIT License (MIT)

Copyright (c) 2014 Redmond Urbino

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.

git repo at git@github.com:redmondurbino/AutoCopyGitHook.git
*/

using UnityEngine;
using System.Collections;
using UnityEditor;
using System.IO;

/// <summary>
/// Automatically copy the pre-commit git hook when Unity does an import
/// </summary>
public class CopyGitHooksPostprocessor :  AssetPostprocessor
{
	/// <summary>
	/// where is the .git Directory located?  The default location assumes .git a sibling to Assets
	/// </summary>
	const string dotGitRelativeDirectoryPath = ".." ;

	/// <summary>
	/// the directory of where the pre-commit file resides.  The default location assumes it's a sibling to Assets
	/// </summary>
	const string hookRelativeDirectoryPath = "..";

	/// <summary>
	/// Name of the hook file to copy, this must be marked as executable.
	/// </summary>
	const string hookFilename = "pre-commit";

	static void OnPostprocessAllAssets (
			string [] importedAssets,
			string [] deletedAssets,
			string [] movedAssets,
			string [] movedFromAssetPaths)
	{
		// the three parameter version of Path.Combine is unavailable;
		string hookPath = Path.Combine ( Path.Combine (Application.dataPath , hookRelativeDirectoryPath), hookFilename);

		if (File.Exists(hookPath))
		{
			// so we have a hook file, let's see if we have a .git directory
			string gitPath = Path.Combine ( Path.Combine(Application.dataPath, dotGitRelativeDirectoryPath), ".git");
			if ( Directory.Exists( gitPath))
			{
				string gitHooksPath = Path.Combine(gitPath, "hooks");
				if ( ! Directory.Exists( gitHooksPath))
				{
					// sometimes the hooks directory is not automatically created, let's create it
					Directory.CreateDirectory( gitHooksPath);
				}
				string destinationFullPath = Path.Combine(gitHooksPath, hookFilename);
				bool doCopy = true;
				if ( File.Exists(destinationFullPath) )
				{
					// don't copy if the destination file is newer than the source file
					FileInfo destInfo = new FileInfo(destinationFullPath);
					FileInfo srcInfo = new FileInfo(hookPath);
					if ( srcInfo.CreationTimeUtc <= destInfo.CreationTimeUtc)
					{
						doCopy = false;
					}
					else
					{
						File.Delete( destinationFullPath);
					}
				}
				if (doCopy)
				{
					File.Copy( hookPath, destinationFullPath);
					Debug.Log ( "git hook copied from " + hookPath + " to " + destinationFullPath);
				}
			}
			else
			{
				Debug.LogWarning(".git directory does not exist at" + gitPath);
			}
		}
		else
		{
			Debug.LogWarning(hookPath + " does not exist");
		}
	}
}
