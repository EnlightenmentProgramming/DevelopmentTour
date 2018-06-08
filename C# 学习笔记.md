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

## C# 常用数据结构

* 数组Array：

  * 数组存储在连续的内存上
  * 数组的内容都是相同的类型
  * 数组可以直接通过下标访问

  ```c#
  int size = 5;
  int[] test = new int[size];
  ```

  ☆★ 创建一个新的数组时将在CLR托管堆中分配一块连续的内存空间，来盛放数量为size，类型为所声明类型的数组元素，如果类型为值类型，则将会有size哥未装箱的该类型的值被创建。如果类型为引用类型，则将会有size个相应类型的引用被创建

  ​	由于是在连续内存上存储的，所以它的索引速度非常快，访问一个元素的时间是恒定的也就是说与元素数量无关，而且赋值与修改元素也很简单

  ```C#
  string[] test2 = new string[3]
  //赋值
  test2[0] = "hello";
  test2[1] = "world";
  test2[2] ="!";
  //修改
  test2[0]="hello world !";
  ```

  ​	但是有有点，那么久一定会伴随着缺点。由于是连续的存储，所以在两个元素之间插入新的元素就变得不方便。而且就像上面的代码所示的那样，声明一个新的数组时，必须指定其长度，这就会存在一个潜在的问题，那就是当我们声明的长度过长时，显然会浪费内存，当我们声明长度过短时，则面临着溢出的风险。这就使得写代码像是投机。小匹夫很厌恶这样的行为，针对这中缺点，下面隆重推出ArrayList.

* ArrayList：为了解决数组创建时必须指定长度以及只能存放相同类型的缺点而退出的数据结构。ArrayList是System.Collections命令空间下的一部分，所以若要使用则必须引入System.Collections。正如上文所说，ArrayList解决了数组的一些缺点

  * 1.不必在声明ArrayList时指定长度，这是由于ArrayList对象的长度是按照其中存储的数据来动态增长与缩减的
  * 2.ArrayList可以存储不同类型的元素。这是由于ArrayList会把它的元素当作Object来处理。因而，加入不同类型的元素是允许的

  说了那么一堆“优点”，也该说说缺点了吧。为什么要给“优点”打上引号呢？那是因为ArrayList可以存储不同类型数据的原因是由于把所有的类型都当作Object来处理，也就是说ArrayList的元素其实都是Object类型的，那么问题就来了

  * 1.ArrayList不是类型安全的。因为把不同的类型都当作Object来处理，很有可能会在使用ArrayList时发生类型不匹配的情况
  * 2.如上所述，数组存储值类型时并未发生装箱，但是ArrayList由于把所有类型都当作Object，所以不可避免的当插入值类型时会发生装箱操作，在索引取值时会发生拆箱操作，这能忍吗？

  ★ 为何说频繁的没有必要的装箱和拆箱不能忍呢？且听小匹夫慢慢道来：所谓装箱（boxing）：就是值类型实例到对象的转换。那么拆箱：就是将引用类型转换为值类型

* List<T> 泛型List

  ​	为了解决ArrayList不安全类型与装箱拆箱的确定，所以出现了泛型的概念，作为一种新的数组类型引入。也是工作中经常用到的数组类型。和ArrayList很相似，长度都可以灵活的改变，最大的不同在于在声明List集合时，我们同时需要为其声明List集合内数据的对象类型，这点又和Array很相似，其实List<T>内部使用了Array来实现。

  这么做最大的好处就是

  * 1.即确保了类型安全
  * 2.也取消了装箱和拆箱的操作
  * 3.它融合了Array可以快速访问的优点以及ArrayList长度可以灵活变化的优点

* LinkedList<T> ：也就是链表了。和上述的数组最大的不同之处就是在于链表在内存存储的排序上可能是不连续的。这是由于链表是通过上一个元素指向下一个元素来排列的，所以可能不能通过下表来访问。

  既然链表最大的特点就是存储在内存的空间不一定连续，那么链表相对于数组最大优势和劣势就显而易见了。

  * 1.向链表中插入或删除节点无需调整结构和容量。因为本身不是连续存储而是靠各对象的指针所决定，所以添加元素和删除元素都要比数组要有优势。

  * 2.链表适合在需要有序的情境下添加新的元素，这里还拿数组做对比，例如要在数组中间某个位置添加新的元素，则可能需要移动很多元素，而对于链表而言可能只是若干元素的只想发生变化而已。

  * 3.有优点就有缺点，由于其在内存中不一定是连续排列，所以访问时候无法利用下标，而是必须从头节点开始，逐次便利下一个节点直到寻找到目标。所以当需要快速访问对象时，数组无疑更有优势。

    综上，链表适合元素数量不固定，需要经常增减节点的情况。

* Queue<T> 在Queue<T>这种数据结构中，最先插入的元素将是最先被删除；反之最后插入的元素将最后被删除，因此队列又称为“先进先出”（FIFO--first in first out）的线性表。通过使用Enqueue和Dequeue这两个方法来实现对Queue<T>的存取

  一些需要注意的地方：

  1.先进先出的场景。

  2.默认情况下，Queue<T>的初始容量为32，增长因子为2.0.

  3.当使用Enqueue时，会判断队列的长度是否足够，如不足，则依据增长因子来增加容量，例如当为初始的2.0时，则队列容量增长2倍。

  4.乏善可陈。

* Stack<T> 与Queue<T> 相对，当需要使用后进先出顺序（LIFO）的数据结构时，我们就需要用到Stack<T>了。

  * 后进先出的情景
  * 默认容量为10
  * 使用pop和push来操作
  * 乏善可陈

* Dictionary<K,T> 

  * 以空间换时间，通过更多的内存开销来满足我们对速度的追求

* 几种数据结构的使用场景

  | Array           | 需要处理的元素数量确定并且需要使用下标时可以考虑，不过建议使用List<T> |
  | --------------- | ------------------------------------------------------------ |
  | ArrayList       | 不推荐使用，建议用List<T>                                    |
  | List<T>         | 需要处理的元素数量不确定时通常建议使用                       |
  | LinkedList<T>   | 链表适合元素数量不固定，需要经常增减节点的情况，2端都可以增减 |
  | Queue<T>        | 先进先出的情况                                               |
  | Stack<T>        | 后进先出的情况                                               |
  | Dictionary<K,T> | 需要键值对，快速操作                                         |

* C# 中的string

  * string是.Net中String类型的别名，是引用类型。String对象是不可改变的，每次使用System.String类中的方法之一时，都要在内存中创建一个新的字符串对象，这就需要为该新对象分配新的空间。在需要对字符串执行重复修改的情况下，与创建新的String对象相关的系统开销可能会非常昂贵。如果要修改字符串而不创建新的对象，则可以使用System.Text.StringBuilder类。
  * StringBuilder 对象时动态对象，允许扩充它所封装的字符串中字符的数量，但是可以为它容纳的最大字符数指定一个值，此值称为该对象的容量，不应将它与当前StringBuilder对象容纳的字符串长度混淆在一起。



























 	