
Vue.filter("dateFormat",
    (val, format) => {
        format = format || "YYYY-MM-DD hh:mm:ss";
        return moment(val).format(format);
	});

// Format Currency to $12.34
Vue.filter("formatCurrency",
	(val) => {
	    var n = val || 0;
	    if (isNaN(val)) {
	        n = 0;
	    }

	    n = n.toString().replace(/\$|\,/g, "");
	    var sign = (n == (n = Math.abs(n)));
	    n = Math.floor(n * 100 + 0.50000000001);
	    var cents = n % 100;
	    n = Math.floor(n / 100).toString();
	    if (cents < 10)
	        cents = "0" + cents;
	    for (var i = 0; i < Math.floor((n.length - (1 + i)) / 3) ; i++)
	        n = n.substring(0, n.length - (4 * i + 3)) + "," + n.substring(n.length - (4 * i + 3));
	    return ((sign ? "" : "-") + "$" + n + "." + cents);
    });

// Format Currency to $12.345
Vue.filter("formatShareCurrency",
    (val) => {
        var n = val || 0;
        if (isNaN(val)) {
            n = 0;
        }

        n = n.toString().replace(/\$|\,/g, "");
        var sign = (n == (n = Math.abs(n)));
        n = Math.floor(n * 1000 + 0.50000000001);
        var cents = n % 1000;
        n = Math.floor(n / 1000).toString();
        if (cents < 10) {
            cents = "00" + cents;
        }
        else if (cents < 100){
            cents = "0" + cents;
        }
        for (var i = 0; i < Math.floor((n.length - (1 + i)) / 3); i++)
            n = n.substring(0, n.length - (4 * i + 3)) + "," + n.substring(n.length - (4 * i + 3));
        return ((sign ? "" : "-") + "$" + n + "." + cents);
    });

Vue.filter("formatNumber",
    (val, decimal, prefix, postfix) => {
        decimal = decimal || 2;
        prefix = prefix || "";
        postfix = postfix || "";

        let v = (val === null || val === undefined ? "" : val + "")
            .toString()
            .trim();
        if (v.length === 0) v = "0";

        let parts = (+v).toString().split(".");
        parts[0] = parts[0].replace(/\B(?=(\d{3})+(?!\d))/g, ",");
        if (decimal > 0)
            parts[1] = (parts[1] || "").padEnd(decimal, 0).substr(0, decimal);

        return prefix + parts.join(".") + postfix;
    });

// Format TFN
Vue.filter("formatTFN",
	(val) => {
	    return formatTFN(val || "");
	});

Vue.filter("toPercent",
    (val) => {
        if (!val || isNaN(val))
            return "0.00%"

        return (Math.round(val * 10000) / 100).toFixed(2) + '%'
    });

Vue.filter("toRatePercent",
    (val) => {
        if (!val || isNaN(val))
            return "0.00%";

        if (val > 1)
            return val.toFixed(2) + '%';

        return (Math.round(val * 10000) / 100).toFixed(2) + '%';
    });
