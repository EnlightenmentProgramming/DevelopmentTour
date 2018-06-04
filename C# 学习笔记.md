# C# 学习笔记

## 1.历史版本

| 语言版本    | 发布时间 | .Net framework要求        | vISUAL STUDIO版本        |
| ----------- | -------- | ------------------------- | ------------------------ |
| C# 1.0      | 2002.1   | .Net Framework 1.0        | Visual Studio .Net 2002  |
| C#  1.1/1.2 | 2003.4   | .Net Framework 1.1        | Visual Studio .Net 2003  |
| C# 2.0      | 2005.11  | .Net Framework 2.0        | Visual Studio 2005       |
| C# 3.0      | 2007.11  | .Net Framework2.0/3.0/3.5 | Visual Studio 2008       |
| C# 4.0      | 2010.4   | .Net Frameworrk 4.0       | Visual Studio 2010       |
| C# 5.0      | 2012.8   | .Net Framework 4.5        | Visual Studio 2012/2013  |
| C# 6.0      | 2015.7   | .Net Framework 4.6        | Visual Studio 2015       |
| C# 7.0      | 2017.3   | .Net Framework 4.6.2`     | Visual Studio 2017       |
| C# 7.1      | 2017.6   | .Net Framework            | Visual Studio 2017 v15.3 |
| C# 8.0      | 待发布   | .Net Framework 4.7.1      | Visual Studio 2017 v15.7 |

## C# 1.0 特性

> ​	第一个版本，编程语言最基础的特性

* Classes: 面向对象特性，支持类类型
* Structs:结构
* Interfaces:接口
* Events:事件
* Properties:属性，类的成员，提供访问字段的灵活方法
* Delegates:委托是一种引用，表示对具有特定参数列表和返回类型的方法的引用
* Exprssions,Statements,Operators: 表达式，语句，操作符
* Attributes: 特性，为程序代码添加元数据或声明性信息，运行时，通过反射可以访问特性信息
* Literals:字面值（或理解为常量值），区别常量，常量是和变量相对的

## C# 2特性（vs 2005）

* Generics:泛型
* Anonymous Methods: 匿名方法
* Partial types:分部类型，可以将类、接口、结构等类型定义拆分到多个文件内
* Iterators:迭代器
* Nullable Types:可以为null的类型，该类可以是其他值或者null
* Gettter/Setter Separate Accessibility: 属性访问控制
* Method group converisions (delegates): 方法组转化，可以将生命委托代表一组方法，隐士调用
* Static Classes:静态类
* Delegate Inference:委托推断，可以将方法名赋值给委托变量

## C# 3特性（vs 2008）

* Implicity typed local variables:
* Object and collection initalizers:对象和集合初始化
* Auto-Implemented propertities:自动属性，自动生成属性方法，生命更简洁
* Anonymous types: 匿名类型
* Extrnsion methods:扩展方法
* Query experssions:查询表达式
* Lambda expressions: Lambda表达式
* Expression trees:树形表达式，是以树形数据结构表示代码，是一种新数据类型
* Partial methods:部分方法

## C# 4特性（vs 2010）

* Dynamic binding:动态绑定
* Named and optional arguments:命名参数和可选参数
* Generic co-and contranvariance:泛型的协变和逆变
* Embedded interop types("NoPIA"):开启嵌入类型信息，增加引用COM组件程序的中立性

## C# 5特性（vs 2012）

* Asynchronous methods: 异步方法
* Caller info attributes:调用发信息特性，调用时访问调用者信息

## C# 6 特性（vs 2015）

* Complier-as-a-service(Roslyn)
* Import of static members type into namespace:支持导入类中的静态成员
* Exception filters : 异常过滤器
* Await in catch/finally blocks:支持在catch/finally语句块中使用await语句
* Auto property initalizers:自动属性初始化
* Default value for getter-only properties:为只读属性设置默认值
* Expression-bodied members:支持以表达式为主体的成员方法和者只读属性
* Null propagator (null-conditinal operator,succinct null checking):null条件操作符
* String interpolation :字符串插值，产生特殊格式字符串的新方法
* nameof operator:nameof 操作符，返回方法、属性、变量的名称
* Dictionary initializer:字段初始化

## C# 7特性（vs 2017）

* Out variables:out变量直接声明，例如可以out in parameter
* Pattern matching:模式匹配，根据对象类型或者其他属性实现方法派发
* Tupples:元组
* Deconstruction:元组解析
* Discards:没有命名的变量，只是展位，后面的代码不使用它的值
* Local funtions:局部函数
* Binary literals:二进制字面量
* Digit separators:数字分隔符
* Ref returns and locals :引用返回值和局部变量
* Generalized async return types: async中使用泛型返类型
* More expression-bodied members:允许构造器、解析器、属性可以使用表达时作为body
* Throw expression:throw 可以在表达式中出现

## C# 7.1特性（vs 2017 version 15.3）

* Async main :在main 方法中使用Async 方式
* Default experssions:引入新的字面值default
* Reference assemblies:
* Inferred tuple element names:
* Pattern-matching with generics:

 ## C# 8特性（vs 2017 version 15.7）

* Default Interface Methods:缺省接口实现
* Nullable inference type NullableReference Type:非空和可空的数据类型
* Recursive patterns:递归模式
* Async streams:异步数据流
* Caller expression attribute 调用方法表达式属性
* Target-typed new 
* Generic attributes 通用属性
* Ranges
* Default in deconstruction 
* Relax ordering of ref and partial modifiers

## .Net Framework 的组件

* 公共语言运行库（Common Language Runtime -CLR）
* .Net Framework 类库（.Net Framework class library）
* 公共语言规范（Common Language Sepcification）
* 通用类型系统（Common Type System）
* 元数据（Metadata）和组件（Assemblies）
* Windows窗体（Windows Forms）
* Asp.Net 和Asp.Net AJAX
* ADO.Net
* Windows工作流基础（Windows Workflow Foundation-WF）
* Windows 显示基础（Windows Presentation Foundation）
* Windows 通信基础（Windows Comunication Foundation -WCF）
* LINQ

## C# 数据类型

* 值类型(Value Types):派生自System.ValueType类
* 引用类型(Reference Types)
* 指针类型(Pointer Types)
* 当一个值类型转换为对象类型时，则称为装箱，另一方面，当一个对象类型转换为值类型时，则被称为拆箱
* 动态类型Dyamic与对象类型相似，但是对象类型的变量的类型检查是在编译时进行的，动态类型的变量的类型检查是在运行时进行的

## C# 类型转换

* **隐式类型转换:**这些类型转换是C#默认的以安全方式进行的转换，不会造成数据丢失，比如，从小的整数类型转换为大的整数类型，从派生类转换为基类
* **显示类型转换:**即强制类型转换，强制类型转换需要强制转换类型符，而且强制类型转化会造成数据丢失

## C#变量

* 一个变量只不过是一个供程序操作的存储区的名字。在C#中，每一个变量都有一个特定的类型，类型决定了变量的内存大小和布局，范围内的值可以存储在内存中，可以对变量进行一些列的操作。

## C#常量

* 常量是固定值，程序执行期间不会改变。常量可以被当作常规变量，只是它们的值在定义后不能被修改