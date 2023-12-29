
using SpaceInvadersServer;
using System.Net;

// чтобы понять, что происходит, передвигайся по конструкторам и методам по порядку
// типа сейчас посмотри, что делает конструктор GameServer
// и там, когда видишь новый конструктор или метод, залезай в него и т.д.
GameServer gameServer = new(IPAddress.Parse("127.0.0.1"));