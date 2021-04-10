
# Meta.RabbitMQ介绍

[![Meta.RabbitMQ](https://img.shields.io/nuget/v/Meta.RabbitMQ.svg)](https://www.nuget.org/packages/Meta.RabbitMQ)

* 基于.NetStandard 2.0的RabbitMQ的轻量级框架。
* 适配集群，非集群多服务，多VirtualHost场景开发。
* 内置连接池管理，只需要做好配置，无需注入大量RabbitMQ Producer。
* 发送者、订阅者已抽象实现，直接继承或注入引用即可快速开发。
* 支持RabbitMQ Header传输
* 支持死信队列Expiration传入
---

# 如何开始？

* 引用Nuget包 [Meta.RabbitMQ](https://www.nuget.org/packages/Meta.RabbitMQ)

## 配置发送者Procucer
### Startup.cs
``` C#
public void ConfigureServices(IServiceCollection services)
{
    services.Configure<RabbitMQOptionCollection>(a =>
    {
        //添加配置, 添加多个时使用Name标志
        //a.TryAdd() 方式根据Name忽略相同配置
        a.Add(new RabbitMQOption
        {
            HostName = "localhost", 
            Port = 5672,
            UserName = "guest",
            Password = "guest",
            VirtualHost = "/",
            //连接配置唯一标志, 默认: ""
            //只有一项配置的情况, 可忽略
            Name = "", 
        });
    });
    //也可使用appsettings.json配置
    //"RabbitMQ": {
    //    "Options": [
    //        {
    //            "HostName": "localhost",
    //            "Port": 5672,
    //            "UserName": "guest",
    //            "Password": "guest",
    //            "VirtualHost": "test_mq",
    //            "MaxChannelPoolSize": 5000,
    //            "Name": ""
    //        }
    //    ]
    //}
    //services.Configure<RabbitMQOptionCollection>(Configuration.GetSection("RabbitMQ"));
    services.AddRabbitMQProducerService();
}
```
### Controller或其他注入类
``` C#
public class SomeController : Controller
{
    private readonly Meta.RabbitMQ.Producer.IMessageProducer _messageProducer;
    public SomeController(Meta.RabbitMQ.Producer.IMessageProducer messageProducer)
    {
        _messageProducer = messageProducer;
    }

    [HttpGet]
    public async Task SomeAction()
    {
        var header = new Dictionary<string, string>()
        {
            [Meta.RabbitMQ.Generic.Headers.Exchange] = "test.ex.v1", //交换机
            [Meta.RabbitMQ.Generic.Headers.RoutingKey] = "test.rk.v1", //路由key
            [Meta.RabbitMQ.Generic.Headers.Name] = "", //配置中的Name, 如果单一配置可忽略
            // ...
            //以上就是发送者必填参数, 更多参数见Meta.RabbitMQ.Generic.Headers
        };
        //Message<T>是单个消息
        //Messages<T>是消息集合, 开启一次相同配置的通道发送多条消息
        //可以是字符串, 实体类等, 根据ISerializer规则序列化
        var testMessage = new Message<string>(header, message);
        await _messageProducer.SendAsync(testMessage);
    }
}
```
---
## 配置订阅者Subscriber
### Subscriber.cs

``` C#
//ConsumerSubscriberBase<T>已经封装了一层
//T: 支持大部分常用类型, string class等
//也可直接继承IConsumerSubscriber
public class SimpleSubscriber : ConsumerSubscriberBase<string>
{
    public string Exchange => "test.ex.v1"; //交换机
    public string RoutingKey => "test.rk.v1"; //路由key
    public string ExchangeType => RabbitMQ.Client.ExchangeType.Direct; //交换机类型
    public string Queue => "test.queue.v1"; //订阅队列
    public string Name => ""; //Startup.cs配置的Name, 单一配置可忽略
    //开启线程数量, 当值为0时取全局配置, 默认为0
    public override ushort ThreadCount => 20;
    //同时订阅线程数, 0则无限制
    public override ushort PrefetchCount => 0; 
    //抛出异常是否也确认消费消息, 默认: true, 选择false可能会产生大量日志, 清谨慎选择
    public override bool CommitIfAnyException => true; 
    //订阅者配置
    public override ClientOption ClientOption => new ClientOption(Exchange, RoutingKey, ExchangeType, Queue, Name); 
    public override Task Invoke(Message<string> message)
    {
        // todo: 数据处理逻辑
        return Task.CompletedTask;
    }
}
```
### Startup.cs
``` C#
public void ConfigureServices(IServiceCollection services)
{
    //此处与配置Producer一致
    services.Configure<RabbitMQOptionCollection>(a =>
    {
        a.Add(new RabbitMQOption
        {
            HostName = "localhost", 
            Port = 5672,
            UserName = "guest",
            Password = "guest",
            VirtualHost = "/",
            Name = "", 
        });
    });
    services.AddSingleton<IConsumerSubscriber, SimpleSubscriber>();
    services.AddRabbitMQConsumerHostedService(a =>
    {
        a.SubscribeThreadCount = 0; //全局订阅者线程数设置
        a.ConsumerReceiveFilter<CustomReceivedFilter>(); //见下面自定义设置
    });
    //也可选择此种注入方式, 与上面不同的是需要手动注册/关闭消费者 
    //services.AddRabbitMQConsumer();
}
//此处用于某些服务使用AddScoped注入的场景
//使用AddRabbitMQConsumer手动注册/关闭消费者
//public void Configure(IHostApplicationLifetime hostApplicationLifetime, IConsumerRegister consumerRegister)
//{
//    hostApplicationLifetime.ApplicationStarted.Register(consumerRegister.Start);
//    hostApplicationLifetime.ApplicationStopped.Register(consumerRegister.Dispose);
//}
```

## 自定义设置

### Meta.RabbitMQ.Serialization.ISerializer
* 默认序列化器使用Newtonsoft.Json UTF8格式序列化，如需自定义可重写此接口在注入服务前注入即可。
* 建议发送者与订阅者使用同一个序列化器，否则会有协议问题
* 详细可参考[Meta.RabbitMQ.Serialization.DefaultSerializer](/src/Meta.RabbitMQ/Serialization/DefaultSerializer.cs)
``` C#
services.TryAddSingleton<ISerializer, CustomSerializer>();
```
### Meta.RabbitMQ.Consumer.IConsumerReceivedFilter
* 提供了消费者的接收处理入口
* 用于可自定义执行逻辑前和抛出异常后的逻辑处理
* 详细可参考[Meta.RabbitMQ.Serialization.ConsumerReceivedFilter](/src/Meta.RabbitMQ/Consumer/ConsumerReceivedFilter.cs)
``` C#
services.AddRabbitMQConsumerHostedService(a =>
{
    a.ConsumerReceiveFilter<CustomReceivedFilter>();
});
```