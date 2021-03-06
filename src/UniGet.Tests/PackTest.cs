﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace UniGet.Tests
{
    public class PackTest
    {
        [Fact]
        private void Test_Simple()
        {
            // Act

            PackTool.Process(new PackTool.Options
            {
                ProjectFile = TestHelper.GetDataPath("Simple.json"),
                OutputDirectory = TestHelper.GetOutputPath()
            });

            // Assert

            var unpackPath = TestHelper.CreateOutputPath("Unpack");
            Extracter.ExtractUnityPackage(TestHelper.GetOutputPath() + "/Simple.1.0.0.unitypackage", unpackPath, "Simple", true, true);

            var packagePath = Path.Combine(unpackPath, "Assets", "UnityPackages", "Simple");
            AssertFileExistsWithMeta(packagePath, "../Simple.unitypackage.json");
            AssertFileExistsWithMeta(packagePath, "Text1.txt");
            AssertFileExistsWithMeta(packagePath, "Text2.txt");
            AssertFileExistsWithMeta(packagePath, "SubDir", "TextInSubDir.txt");

            AssertFileExists(unpackPath, "Assets", "UnityPackages.meta");
            AssertFileExists(unpackPath, "Assets", "UnityPackages", "Simple.meta");
            AssertFileExists(unpackPath, "Assets", "UnityPackages", "Simple", "SubDir.meta");
        }

        [Fact]
        private void Test_FileItem()
        {
            // Act

            PackTool.Process(new PackTool.Options
            {
                ProjectFile = TestHelper.GetDataPath("FileItem.json"),
                OutputDirectory = TestHelper.GetOutputPath()
            });

            // Assert

            var unpackPath = TestHelper.CreateOutputPath("Unpack");
            Extracter.ExtractUnityPackage(TestHelper.GetOutputPath() + "/FileItem.1.0.0.unitypackage", unpackPath, "FileItem", true, true);

            var basePath = Path.Combine(unpackPath, "Assets", "UnityPackages");
            AssertFileExistsWithMeta(basePath, "FileItem.unitypackage.json");
            AssertFileExistsWithMeta(basePath, "1/Text.txt");
            AssertFileExistsWithMeta(basePath, "2/Text.txt");

            AssertFileExists(unpackPath, "Assets", "UnityPackages.meta");
            AssertFileExists(unpackPath, "Assets", "UnityPackages", "1.meta");
            AssertFileExists(unpackPath, "Assets", "UnityPackages", "2.meta");
        }

        [Fact]
        private void Test_InheritBaseProject()
        {
            // Act

            PackTool.Process(new PackTool.Options
            {
                ProjectFile = TestHelper.GetDataPath("InheritChild.json"),
                OutputDirectory = TestHelper.GetOutputPath()
            });

            // Assert

            var unpackPath = TestHelper.CreateOutputPath("Unpack");
            Extracter.ExtractUnityPackage(TestHelper.GetOutputPath() + "/InheritChild.1.0.0.unitypackage", unpackPath, "InheritChild", true, true);

            var packagePath = Path.Combine(unpackPath, "Assets", "UnityPackages", "InheritChild");
            AssertFileExistsWithMeta(packagePath, "../InheritChild.unitypackage.json");
            AssertFileExistsWithMeta(packagePath, "Text1.txt");
            AssertFileExistsWithMeta(packagePath, "Text2.txt");
        }

        [Fact]
        private void Test_MergeDependencies()
        {
            // Arrange

            PackTool.Process(new PackTool.Options
            {
                ProjectFile = TestHelper.GetDataPath("DepA.json"),
                OutputDirectory = TestHelper.GetOutputPath()
            });

            // Act

            PackTool.Process(new PackTool.Options
            {
                ProjectFile = TestHelper.GetDataPath("DepB-Full.json"),
                OutputDirectory = TestHelper.GetOutputPath(),
                LocalRepositoryDirectory = TestHelper.GetOutputPath()
            });

            // Assert

            var unpackPath = TestHelper.CreateOutputPath("Unpack");
            Extracter.ExtractUnityPackage(TestHelper.GetOutputPath() + "/DepB.1.0.0.unitypackage", unpackPath, "DepB", true, true);
        }

        private void AssertFileExists(params string[] names)
        {
            Assert.True(File.Exists(Path.Combine(names)), "File: " + Path.Combine(names));
        }

        private void AssertFileExistsWithMeta(params string[] names)
        {
            Assert.True(File.Exists(Path.Combine(names)), "File: " + Path.Combine(names));
            Assert.True(File.Exists(Path.Combine(names) + ".meta"), "File: " + Path.Combine(names) + ".meta");
        }
    }
}
