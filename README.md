<h1>项目概括</h1>

快速在**Windows**上递归建立文件夹内文件硬链接

<h1>用法</h1>

```
./HardLinkTool.exe <path> [--output/-o] <output path> [--skipSize/-s] <1024> [--overwrite/-r] [--refresh] <1000> [--progress/-p]
```

<h1>参数解释</h1>

````
path: 需要建立硬链接的文件或文件夹.

output 输出的目录名,如果 path 是个文件则是文件名(默认为 path 所在的目录,名字加上 -link).

skipSize: 直接复制小于这个大小的文件.单位为 byte ,默认为 1024;

overwrite: 如果目录里存在 output 目录/文件,是否覆盖.

refresh: 进度显示的刷新时间, 如果是关闭进度显示则无效. 请不要填写负数.

progress: 无效, 目前默认启用进度显示.

除了 path 皆为可选项.
````

<h1>PS</h1>
这个是我想保持bt做种制作的小工具.

我使用了**Windows Api**,其他的操作系统应该没有办法运行.

<h1>特别感谢</h1>

1. [command-line-api](https://github.com/dotnet/command-line-api "使用了这个Nuget包,在项目里实现了命令解析")
2. [戈小荷](https://github.com/gehongyan "感谢大佬一直以来对我的帮助")
3. [Markdown 教程](https://markdown.com.cn/basic-syntax/links.html "现用现查")
