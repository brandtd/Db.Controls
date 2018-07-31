#region MIT License (c) 2018 Dan Brandt

// Copyright 2018 Dan Brandt
//
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and
// associated documentation files (the "Software"), to deal in the Software without restriction,
// including without limitation the rights to use, copy, modify, merge, publish, distribute,
// sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all copies or
// substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT
// NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM,
// DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT
// OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

#endregion MIT License (c) 2018 Dan Brandt

using Newtonsoft.Json;

namespace AR.Network
{
    [JsonObject]
    public class ARConnectionResponse
    {
        [JsonProperty(PropertyName = "arstream_fragment_max_ack_interval", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public int? ArstreamFragmentMaxAckInterval { get; set; }

        [JsonProperty(PropertyName = "arstream_fragment_maximum_number", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public int? ArstreamFragmentMaximumNumber { get; set; }

        [JsonProperty(PropertyName = "arstream_fragment_size", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public int? ArstreamFragmentSize { get; set; }

        [JsonProperty(PropertyName = "c2d_port", Required = Required.Always)]
        public int C2dPort { get; set; }

        [JsonProperty(PropertyName = "c2d_update_port", Required = Required.Always)]
        public int? C2dUpdatePort { get; set; }

        [JsonProperty(PropertyName = "skycontroller_version", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public string SkycontrollerVersion { get; set; }

        [JsonProperty(PropertyName = "status", Required = Required.Always)]
        public int Status { get; set; }
    }
}