# DelphiTask
Task for Delphi  &amp; Related tools for Nant

Contains tasks to do the following:
* <brcc> Task to compile resource file (calls brcc32.exe).
* <versioninfo> Task to generate version information that can be compiled with <brcc>.
* <dcc> Task to compile a Delphi project. Currently supports Delphi 2 - XE7.

```xml
<project name="Project1" default="build">

	<path id="lib">
		<pathelement dir="..\lib\component" />
		<pathelement dir="${delphi::path(7)}\Rave5\Lib\" />
	</path>

	<target name="build" depends="clean, compile"/>

	<target name="clean">
		<echo>Cleaning build files</echo>
		<delete dir="dcus\"/>
		<delete dir="bin\"/>
		<mkdir dir="dcus\" />
		<mkdir dir="bin\" />
	</target>

	<target name="compile" depends="clean">
	
		<versioninfo productname="My Project"
	                 companyname="A Company"
	                 copyright="Copyright (c) A Company 2017"
	                 description="An example project"
	                 target="versioninfo.rc">
			<productversion major="1" minor="0" release="0" build="34"/>
			<fileversion major="1" minor="0" release="0" build="34"/>
		</versioninfo>
	
		<brcc file="versioninfo.rc" version="7"/>
		
		<dcc source="Project1.dpr" 
				version="7" 
				writecfg="true" 
				console="false" 
				debuginfo="exe" 
				exeoutput="bin\" 
				dcuoutput="dcus\" 
				build="true" 
				mapfile="detailed">
			<compileroptions>
			  	<option name="L" value="False"/>
			  	<option name="D" value="False"/>
			  	<option name="Y" value="False"/>
			  	<option name="O" value="True"/>
			  	<option name="W" value="True"/>
				<option name="C" value="True"/>
			</compileroptions>
			<unitpath includedelphilib="true">
				<pathelement dir="source\" />
				<path refid="lib" />
			</unitpath>
			<defines>
				<define name="ADEFINE" />
			</defines>
		</dcc>
	</target>
</project>
```
