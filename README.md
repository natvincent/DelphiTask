# DelphiTask
Task for Delphi  &amp; Related tools for Nant

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
