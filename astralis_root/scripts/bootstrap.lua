
add_var("time_and_weather", {
    start_hour = 11,
    start_minute = 50,
})


on_event("time_change_event", function (event)
    -- log_info("Time changed !")
end)

on_bootstrap(function ()
    log_info("Hello, world!")
    log_info("Ciao da Tommy!")

end)
