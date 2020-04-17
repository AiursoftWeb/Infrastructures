# Aiursoft 版本库 - Git Commit 规范

## 基础规则

### 仅仅Commit相关的修改

commit应该是相关的更改的集合。 例如，修复两个不同的错误应该产生两个单独的commit。 小commit使其他开发人员更容易理解这些更改，并在出现问题时将其回滚。
借助staging area之类的工具和仅分段文件的部分功能，Git可以轻松创建非常细化的commit。

### 尽可能频繁Commit

Commit通常要保持较少的修改，这可以帮助开发者仅关注相关的修改。 此外，它允许更频繁地与其他人分享您的代码。 这样，每个人都可以更容易地定期整合更改，避免合并冲突。 相比之下，较大的提交并且很少分享，会导致发生冲突时很难解决。

### 不要提交未完成的工作

你不应该提交一次未完成的工作。 这并不意味着你必须在提交之前完成一个完整的大功能。 恰恰相反：将功能的实现分割成逻辑块，并记住提前和经常提交。 千万不要仅仅是在一天工作完成后的下班离开办公室时进行提交。 如果你只是因为需要一个干净的工作副本而试图提交（检查分支，引入更改等），请考虑使用Git的“stash”功能。

### 提交前检查代码

即使你非常想这么做，也不要提交一个你认为“完成”了的commit。 彻底测试以确保它确实完成并且没有副作用。 虽然在本地存储库中提交未通过检查的代码只需要你自己反省，但是在push给与他人共享代码时，commit前测试一次代码就显得非常重要。

### 认真编写Commit信息

Commit信息的开始应当是你的修改的简要介绍，且不应超过50字。使用一个空行来分隔Commit的正文。在正文中，给出下列问题的答案：

* Commit的动机是什么
* Commit与之前有何不同

使用一般现在时编写这些信息，不要用“修改了”，“正在修改”等词，以便与git merge等命令生成的消息保持一致。

虽然使用一个git远程服务器可以让你的文件安全的同步，但是千万不要把版本控制的远程服务器当作是备份平台或者是网盘。在进行版本控制时，时刻警醒你的修改是要和别人共享的，它们不仅仅体现在文件本身。

### 随时开启新分支

分支是Git最强大的功能之一。优秀的仓库往往在开始工作时就有简明高效的分支结构。分支结构是帮助避免混淆不同开发线的完美工具。您应该在您的开发工作流程中广泛使用分支结构，针对：新功能，错误修复，想法...

## 格式化规则

- 首字母大写，简短__（不超过50字）__总结

- __总是将第二行留空__

- 在命令中写下你的commit信息：“修正错误”，而不是“正在修正错误”或“修正了错误”。 这个约定与由git merge和git revert等命令生成的提交消息相匹配。

- __空行__

- 如果仍然有更多的段落，则出现在空行之后。
     - 可以使用子弹点，每个子弹点之间都有空行
     - 对于子弹点通常使用连字符或星号，前面是一个Tab或4个空格
     - 使用悬挂式缩进而不是首行缩进

### 优秀的提交信息示例

__例1__ (没有描述，只有概要)

```
  commit 3114a97ba188895daff4a3d337b2c73855d4632d
  Author: [removed]
  Date:   Mon Jun 11 17:16:10 2012 +0100

    Update default policies for KVM guest PIT & RTC timers
```

__例2__ (使用子弹点表达描述)
```
  commit ae878fc8b9761d099a4145617e4a48cbeb390623
  Author: [removed]
  Date:   Fri Jun 1 01:44:02 2012 +0000

    Refactor libvirt create calls

     - Minimize duplicated code for create

     - Make wait_for_destroy happen on shutdown instead of undefine

     - Allow for destruction of an instance while leaving the domain
```

__例3__ (使用大段文本表达描述)

```
  commit 31336b35b4604f70150d0073d77dbf63b9bf7598
  Author: [removed]
  Date:   Wed Jun 6 22:45:25 2012 -0400

    Add CPU arch filter scheduler support

    In a mixed environment of running different CPU architectures,
    one would not want to run an ARM instance on a X86_64 host and
    vice versa.

    This scheduler filter option will prevent instances running
    on a host that it is not intended for.

    The libvirt driver queries the guest capabilities of the
    host and stores the guest arches in the permitted_instances_types
    list in the cpu_info dict of the host.

    The Xen equivalent will be done later in another commit.

    The arch filter will compare the instance arch against
    the permitted_instances_types of a host
    and filter out invalid hosts.

    Also adds ARM as a valid arch to the filter.

    The ArchFilter is not turned on by default.
```