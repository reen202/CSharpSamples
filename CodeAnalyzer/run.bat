cd Exec\bin\Debug
Exec.exe *.* ../../../TEST_FOLDER
Exec.exe Exec.cs ../../../TEST_FOLDER
Exec.exe ../../../TEST_FOLDER /s *.cs
Exec.exe *.cs /r ../../../TEST_FOLDER
Exec.exe Exec.cs *.cs /x ../../../TEST_FOLDER
Exec.exe Exec.cs Analyzer.cs /x /r ../../../TEST_FOLDER
Exec.exe Exec.cs /s ../../../TEST_FOLDER /r /x
Exec.exe ../../../TEST_FOLDER
cd ../../../