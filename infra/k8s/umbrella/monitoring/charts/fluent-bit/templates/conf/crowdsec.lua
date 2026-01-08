function parse_json(tag, timestamp, record)
    local cjson = require "cjson.safe"
    local log = record["log"]
    local data = cjson.decode(log)
    if data == nil then
        -- drop broken logs
        return -1, timestamp, record
    end
    return 0, timestamp, data
end