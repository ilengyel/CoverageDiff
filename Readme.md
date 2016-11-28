# CoverageDiff

A small utility to post-process an [OpenCover XML](https://github.com/OpenCover/opencover) file to only include sequence and branch points that are of interest. This filtered coverage file can still be used as an input of [ReportGenerator](http://danielpalme.github.io/ReportGenerator/) to generate the coverage report just on the relevant code. The main motivation is to be able to quickly see the test coverage quality of a particular code patch or pull request by only showing the coverage details of the code specified by a [Unified Diff](http://www.gnu.org/software/diffutils/manual/html_node/Example-Unified.html#Example-Unified) file.

## Usage

**Options**

    >CoverageDiff.exe --help
    CoverageDiff 1.0.0.0
    
      -c, --coverage    Required. OpenCover tool output as an XML file.
      -d, --diff        Required. Unified format patch file.
      -o, --output      Output file for mutated coverage xml file. If omitted then the coverage
                        file name with a '_diff' suffix added is used.
      --help            Display this help screen.
      --version         Display version information.
      
    >

**Example**

    git diff > sample.diff
    CoverageDiff.exe -c OpenCover.xml -d sample.diff 
    tools\ReportGenerator.exe -reports:OpenCover_diff.xml -targetdir:Html_Summary -reporttypes:Html;XmlSummary
    
## Developer Notes

### Assumptions

It appears that ReportGenerator performs its own coverage calculations based on the sequence and branch points in the open cover xml file. This meant I did not need to recalculate the Summary nodes inside the coverage file, and I use the XmlSummary output from the ReportGenerator tool.

### Support for other formats

Currently only OpenOpen cover for coverage format and Unified Diff for source diff are supported. Other formats could easily be supported by implementing the appropriate interface.

### Roadmap

1. [x] Add a release build.
2. [ ] Add some tests.
3. [ ] Setup appveyor build and use this tool to provide coverage reports on pull requests. *A live example.*
4. [ ] Create a nuget package for easy script deployment.
5. [ ] Add a format selection algorithm to support other coverage formats, if somebody wants to add other format support.
