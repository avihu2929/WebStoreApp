let findLocationByAddress = (searchManager, address) => {
    return new Promise((resolve, reject) => {
        let requestOptions = {
            where: address,
            callback: (answer, userData) => {
                resolve(answer.results[0])
            }
        };
        searchManager.geocode(requestOptions);
    });
}
let locationToPushpin = (loc, index) => {
    return new Microsoft.Maps.Pushpin(loc.location, {
        text: String.fromCharCode(index + 'A'.charCodeAt(0)),
        title: loc.address.addressLine + "," + loc.address.locality
    });
}

let getAddressesPushpins = (map, addresses) => {
    return new Promise((resolve, reject) => {
        Microsoft.Maps.loadModule('Microsoft.Maps.Search', () => {
            let searchManager = new Microsoft.Maps.Search.SearchManager(map);
            let promises = addresses.map(addres => findLocationByAddress(searchManager, addres))
            Promise.all(promises).then(locations => {
                resolve(locations.map((loc, index) => locationToPushpin(loc, index)))
            }).catch(err => { })
        });
    });
}
let displayAddressesOnMap = (map, addresses) => {
    getAddressesPushpins(map, addresses).then(pushpins => {
        let pushpinsLayer = new Microsoft.Maps.Layer();
        pushpinsLayer.add(pushpins);
        map.layers.insert(pushpinsLayer);
        map.setView({
            bounds: Microsoft.Maps.LocationRect.fromShapes(pushpins),
            padding: 100
        });
    }).catch(err => {

    })
}

function loadMapScenario() {
    let map = new Microsoft.Maps.Map(document.getElementById('myMap'), {});
    onMapLoaded(map)
}




