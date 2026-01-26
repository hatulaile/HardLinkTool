<h1>项目概括</h1>

递归建立文件夹内文件硬链接的一个小程序

<h1>使用</h1>


推荐使用带有`Ui`后缀的程序.
<details>
<summary>如果非要使用控制台项目</summary>
<h1>控制台用法</h1>

```
./HardLinkTool.exe <path> [--output/-o] <output path> [--skipSize/-s] <1024> [--overwrite/-r] [--refresh] <1000> [--no-progress/-np] [--no-error-log-file/-ne]
```

<h1>参数解释</h1>

````
path: 需要建立硬链接的文件或文件夹.

output 输出的目录名,如果 path 是个文件则是文件名(默认为 path 所在的目录,名字加上 -link).

skipSize: 直接复制小于这个大小的文件.单位为 byte ,默认为 1024;

overwrite: 如果目录里存在 output 目录/文件,是否覆盖.

refresh: 进度显示的刷新时间, 如果是关闭进度显示则无效. 请不要填写负数.

no-progress: 是否不启用进度显示.

no-error-log-file: 是否不输出错误日志.

除了 path 皆为可选项.
````
</details>

<h1>TODO</h1>

- [ ] 更换程序图标
- [ ] 更改致谢按钮位置与样式
- [ ] 增加更多系统支持?
- [ ] 或者更多功能...

<h1>运行截图</h1>
<details>
<summary>截图折叠</summary>
<img width="928" height="517" alt="runing" src="https://github.com/user-attachments/assets/7e7eaa40-5058-49e5-a470-56afc50dd886" />
<img width="928" height="517" alt="complete" src="https://github.com/user-attachments/assets/63f9b435-6554-4bd8-add5-9296c4280319" />
<img width="1540" height="911" alt="image" src="https://github.com/user-attachments/assets/31950dea-9678-4f93-b977-fe5370cb6bc9" />
</details>

<h1>特别感谢</h1>

1. [command-line-api](https://github.com/dotnet/command-line-api "使用了这个Nuget包,在项目里实现了命令解析")
2. [戈小荷](https://github.com/gehongyan "感谢大佬一直以来对我的帮助")
3. [Markdown 教程](https://markdown.com.cn/basic-syntax/links.html "现用现查")
