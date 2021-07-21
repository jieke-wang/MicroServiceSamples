namespace ProductService.Options {
    public class ConsulOptions {
        public string IP { get; set; }
        public int Port { get; set; }
        public string Datacenter { get; set; }
        public string Token { get; set; }
    }
}